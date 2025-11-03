import { Component, signal, OnInit, inject, OnDestroy, ElementRef, ViewChild, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';
import { Router, RouterModule } from '@angular/router';
import { SimulationService, IAResponse } from '../../services/simulation.service';
import { TokenService } from '../../../../core/services/token.service';
import { Contexto, getEdadFromPromptSeed } from '../../../../shared/models/contexto.model';

// Modelo local para mensajes en pantalla
interface Message {
  id: string;
  text: string;
  sender: 'user' | 'ai';
}

// Estado más detallado
type InterviewStatus = 'stopped' | 'recording' | 'waiting_for_ai' | 'finished';

@Component({
  selector: 'app-simulation',
  standalone: true, // Asumiendo standalone por los imports
  imports: [...MATERIAL_IMPORTS, CommonModule, RouterModule],
  templateUrl: './simulation.component.html',
  styleUrl: './simulation.component.scss'
})
export class SimulationComponent implements OnInit, OnDestroy {
  // Inyección de servicios
  private router = inject(Router);
  private simulationService = inject(SimulationService);
  private tokenService = inject(TokenService);
  private cdr = inject(ChangeDetectorRef); // Para forzar detección de cambios

  // Elemento para scroll
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;

  // Estado de la Simulación
  interviewStatus = signal<InterviewStatus>('stopped');
  messages = signal<Message[]>([]);
  aiSpeaking = signal(false); // Para el indicador de "escribiendo"

  // Datos de la entrevista
  contexto = signal<Contexto | null>(null);
  entrevistaId = signal<number | null>(null);

  // --- Lógica de Grabación ---
  private mediaRecorder: MediaRecorder | null = null;
  private audioChunks: Blob[] = [];
  private audioPlayer = new Audio();
  private messageCounter = 0;

  ngOnInit(): void {
    const contextoString = sessionStorage.getItem('selectedContexto');
    if (!contextoString) {
      // Si no hay contexto, no podemos estar aquí
      this.router.navigate(['/scenarios']);
      return;
    }
    
    this.contexto.set(JSON.parse(contextoString));

    // Configurar el audio player para cuando termine de hablar
    this.audioPlayer.onended = () => {
      this.aiSpeaking.set(false);
      // Habilitar el botón de grabar de nuevo
      this.interviewStatus.set('stopped');
      this.cdr.detectChanges(); // Forzar actualización
    };
  }

  ngOnDestroy(): void {
    // Limpiar el contexto al salir
    sessionStorage.removeItem('selectedContexto');
    // Detener cualquier grabación o audio
    this.mediaRecorder?.stop();
    this.audioPlayer.pause();
  }

  // --- Control de Grabación (Solución Manual) ---

  async startRecording(): Promise<void> {
    if (this.interviewStatus() !== 'stopped') return;

    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      this.mediaRecorder = new MediaRecorder(stream);
      
      this.mediaRecorder.ondataavailable = (event) => {
        this.audioChunks.push(event.data);
      };

      this.mediaRecorder.onstop = () => {
        const audioBlob = new Blob(this.audioChunks, { type: 'audio/wav' });
        this.audioChunks = []; // Limpiar para la próxima grabación
        
        // Enviar el audio al backend
        this.processAudio(audioBlob);
        
        // Detener el stream de medios
        stream.getTracks().forEach(track => track.stop());
      };

      this.mediaRecorder.start();
      this.interviewStatus.set('recording');

    } catch (err) {
      console.error('Error al acceder al micrófono:', err);
      // Aquí deberías mostrar un error al usuario (ej. un Snackbar)
    }
  }

  stopRecording(): void {
    if (this.interviewStatus() !== 'recording') return;
    this.mediaRecorder?.stop();
    // onstop() se encargará del resto
  }

  // --- Lógica de API ---

  private processAudio(audioBlob: Blob): void {
    this.interviewStatus.set('waiting_for_ai');
    const contexto = this.contexto()!;
    const formData = new FormData();
    formData.append('audio', audioBlob, 'recording.wav');
    
    // Los context_traits son el promptSeed
    formData.append('contextTraits', contexto.promptSeed || '');

    const currentEntrevistaId = this.entrevistaId();

    if (currentEntrevistaId === null) {
      // --- TURNO 1: Iniciar Entrevista ---
      const usuarioId = this.tokenService.getNameIdentifier();
      if (!usuarioId) {
        console.error('Error fatal: Usuario no logueado.');
        this.interviewStatus.set('stopped');
        return;
      }
      
      formData.append('usuarioId', usuarioId.toString());
      formData.append('contextoId', contexto.id!.toString());
      formData.append('titulo', contexto.nombre || 'Entrevista de simulación');
      formData.append('duracionMin', '0'); // El backend lo actualizará al final

      this.simulationService.iniciarEntrevista(formData).subscribe(response => {
        this.handleAIResponse(response);
      });

    } else {
      // --- TURNOS > 1: Continuar Diálogo ---
      formData.append('entrevistaId', currentEntrevistaId.toString());
      formData.append('turno', (this.messages().length + 1).toString());
      formData.append('sender', 'User'); // El backend asume que el audio es del 'User'

      this.simulationService.continuarDialogo(formData).subscribe(response => {
        this.handleAIResponse(response);
      });
    }
  }

  private handleAIResponse(response: IAResponse | null): void {
    if (!response || !response.success) {
      console.error('Error en la respuesta de la IA:', response?.error);
      this.interviewStatus.set('stopped');
      // Mostrar error al usuario
      return;
    }

    // 1. Guardar el ID si es la primera llamada
    if (this.entrevistaId() === null) {
      this.entrevistaId.set(Number(response.session_id));
    }

    // 2. Añadir mensaje del usuario (transcripción)
    this.addUserMessage(response.transcription);

    // 3. Añadir mensaje de la IA (respuesta)
    this.addAIMessage(response.response_text);
    
    // 4. Reproducir el audio de la IA
    this.playAudio(response.audio_base64);
  }

  private playAudio(base64: string): void {
    const audioSrc = `data:audio/wav;base64,${base64}`;
    this.audioPlayer.src = audioSrc;
    this.audioPlayer.play();
    this.aiSpeaking.set(true);
  }

  // --- Métodos de la Vista (Helpers) ---

  stopInterview(): void {
    this.interviewStatus.set('finished');
    // Detener todo
    this.mediaRecorder?.stop();
    this.audioPlayer.pause();
    
    // Aquí también deberías llamar a un endpoint para
    // actualizar el 'fecha_cierre' y 'duracion_min' de la Entrevista
    
    this.router.navigate(['/history-detail/', this.entrevistaId()]);
  }

  getStatusIcon(): string {
    switch (this.interviewStatus()) {
      case 'recording': return 'fiber_manual_record';
      case 'waiting_for_ai': return 'hourglass_empty';
      case 'finished': return 'check_circle';
      default: return 'mic_none'; // 'stopped'
    }
  }

  getStatusText(): string {
    switch (this.interviewStatus()) {
      case 'recording': return 'Grabando...';
      case 'waiting_for_ai': return 'Procesando...';
      case 'finished': return 'Finalizado';
      default: return 'Presiona Iniciar para grabar';
    }
  }

  getAINombre(): string {
    return this.contexto()?.nombre || 'IA';
  }

  getAIDescripcion(): string {
    const ctx = this.contexto();
    if (!ctx) return 'Cargando...';
    const edad = getEdadFromPromptSeed(ctx);
    const desc = ctx.descripcion || 'Iniciando simulación...';
    return edad !== 'N/A' ? `${desc} (Paciente: ${edad})` : desc;
  }

  private addUserMessage(text: string): void {
    const message: Message = { id: `msg-${++this.messageCounter}`, text, sender: 'user' };
    this.messages.update(msgs => [...msgs, message]);
    this.scrollToBottom();
  }

  private addAIMessage(text: string): void {
    const message: Message = { id: `msg-${++this.messageCounter}`, text, sender: 'ai' };
    this.messages.update(msgs => [...msgs, message]);
    this.scrollToBottom();
  }

  private scrollToBottom(): void {
    try {
      setTimeout(() => {
        this.messagesContainer.nativeElement.scrollTop = this.messagesContainer.nativeElement.scrollHeight;
      }, 100);
    } catch (err) {}
  }

  trackByMessageId(index: number, message: Message): string {
    return message.id;
  }
}