import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MATERIAL_IMPORTS } from '../../../../shared/material/material';

interface Message {
  id: string;
  text: string;
  sender: 'user' | 'ai';
}

type InterviewStatus = 'stopped' | 'recording' | 'paused' | 'finished';

@Component({
  selector: 'app-simulation',
  imports: [...MATERIAL_IMPORTS, CommonModule],
  templateUrl: './simulation.component.html',
  styleUrl: './simulation.component.scss'
})
export class SimulationComponent {
  interviewStatus = signal<InterviewStatus>('stopped');
  messages = signal<Message[]>([]);
  aiSpeaking = signal(false);
  
  private messageCounter = 0;
  private simulationTimer?: number;

  getStatusIcon(): string {
    switch (this.interviewStatus()) {
      case 'recording': return 'fiber_manual_record';
      case 'paused': return 'pause';
      case 'finished': return 'check_circle';
      default: return 'stop';
    }
  }

  getStatusText(): string {
    switch (this.interviewStatus()) {
      case 'recording': return 'Grabando';
      case 'paused': return 'Pausado';
      case 'finished': return 'Finalizado';
      default: return 'Detenido';
    }
  }

  startInterview(): void {
    this.interviewStatus.set('recording');
    this.simulateConversation();
  }

  pauseInterview(): void {
    this.interviewStatus.set('paused');
    if (this.simulationTimer) {
      clearTimeout(this.simulationTimer);
    }
  }

  stopInterview(): void {
    this.interviewStatus.set('finished');
    if (this.simulationTimer) {
      clearTimeout(this.simulationTimer);
    }
    this.aiSpeaking.set(false);
  }

  private simulateConversation(): void {
    if (this.interviewStatus() !== 'recording') return;

    // Mensaje inicial de la madre buscando ayuda
    if (this.messages().length === 0) {
      this.addAIMessage('Buenos días, doctor. Muchas gracias por recibirme. Vengo porque estoy muy preocupada por mi hijo Diego, tiene 6 años y... bueno, no sé si es normal su comportamiento. Mi familia dice que solo es berrinchudo, pero yo siento que algo más está pasando.');
    }

    // Simular diálogo clínico después de un tiempo
    this.simulationTimer = window.setTimeout(() => {
      if (this.interviewStatus() === 'recording') {
        this.simulateClinicalDialogue();
      }
    }, 7000);
  }

  private simulateClinicalDialogue(): void {
    const clinicalDialogue = [
      {
        interviewer: '¿Podría contarme más específicamente qué comportamientos le preocupan de Diego?',
        mother: 'Bueno, él no me mira a los ojos cuando le hablo, y cuando trato de abrazarlo se pone muy rígido. También hace movimientos raros con las manos, como si estuviera aplaudiendo, especialmente cuando ve luces brillantes o escucha música.'
      },
      {
        interviewer: '¿Desde cuándo ha notado estos comportamientos? ¿Han cambiado con el tiempo?',
        mother: 'Empecé a notarlo más o menos a los 2 años. Al principio pensé que era tímido, pero ahora a los 6 años es más evidente. Antes decía algunas palabras como "mamá" y "agua", pero ahora casi no habla. Solo repite frases de sus caricaturas favoritas.'
      },
      {
        interviewer: '¿Cómo es un día típico con Diego? ¿Hay rutinas específicas que prefiera?',
        mother: 'Ah sí, eso es algo muy marcado. Tiene que desayunar en el mismo plato azul, sentarse en la misma silla, y si cambio algo se pone muy alterado. Llora y se tira al suelo. También le gusta ordenar sus juguetes por colores, una y otra vez.'
      },
      {
        interviewer: '¿Ha consultado con algún otro profesional antes? ¿Qué le han dicho?',
        mother: 'El pediatra dice que cada niño se desarrolla a su ritmo, que no me preocupe. Pero la maestra del jardín me comentó que Diego no juega con otros niños y que cuando hay ruidos fuertes se tapa los oídos y llora. Por eso decidí venir aquí.'
      },
      {
        interviewer: '¿Cómo está la situación familiar? ¿Vive con el papá de Diego?',
        mother: 'No, estoy sola con Diego. Su papá nos dejó el año pasado, decía que no podía con los "berrinches" de Diego. Trabajo medio tiempo en una tienda para poder cuidarlo. Mi mamá a veces me ayuda, pero ella también piensa que solo necesita más disciplina.'
      },
      {
        interviewer: '¿Ha notado si Diego tiene alguna fortaleza o habilidad especial?',
        mother: 'Sí, tiene una memoria increíble. Se sabe todas las canciones de sus programas favoritos y puede armar rompecabezas muy rápido. También reconoce letras y números, aunque no habla mucho. Es como si entendiera todo pero no pudiera expresarlo.'
      },
      {
        interviewer: 'Basándome en lo que me cuenta, creo que sería importante hacer una evaluación más detallada. ¿Estaría dispuesta a que Diego sea visto por un equipo especializado?',
        mother: '¿Usted cree que realmente hay algo? Tengo miedo de que me digan que es mi culpa, que no sé criarlo. Pero también quiero ayudarlo. Si necesita algún tipo de terapia o tratamiento, ¿cómo podría pagarlo? Mi seguro es muy básico.'
      }
    ];

    const currentDialogue = Math.floor(this.messageCounter / 2);
    
    if (currentDialogue < clinicalDialogue.length) {
      const dialogue = clinicalDialogue[currentDialogue];
      
      // Pregunta del entrevistador/estudiante
      this.addUserMessage(dialogue.interviewer);

      // Simular que la madre está pensando
      setTimeout(() => {
        this.aiSpeaking.set(true);
      }, 2000);

      // Respuesta de la madre después de un delay
      setTimeout(() => {
        this.addAIMessage(dialogue.mother);
        this.aiSpeaking.set(false);
        
        // Continuar el diálogo
        if (currentDialogue < clinicalDialogue.length - 1) {
          this.simulateConversation();
        } else {
          // Mensaje final de cierre de la entrevista
          setTimeout(() => {
            this.addAIMessage('Doctor, muchas gracias por escucharme y no juzgarme. Me siento más tranquila sabiendo que hay pasos que podemos seguir para ayudar a Diego. ¿Cuándo podríamos empezar con la evaluación?');
          }, 3000);
        }
      }, 5000);
    }
  }

  private addUserMessage(text: string): void {
    const message: Message = {
      id: `msg-${++this.messageCounter}`,
      text,
      sender: 'user'
    };
    this.messages.update(msgs => [...msgs, message]);
    this.scrollToBottom();
  }

  private addAIMessage(text: string): void {
    const message: Message = {
      id: `msg-${++this.messageCounter}`,
      text,
      sender: 'ai'
    };
    this.messages.update(msgs => [...msgs, message]);
    this.scrollToBottom();
  }

  private scrollToBottom(): void {
    setTimeout(() => {
      const container = document.querySelector('.messages-container');
      if (container) {
        container.scrollTop = container.scrollHeight;
      }
    }, 100);
  }

  trackByMessageId(index: number, message: Message): string {
    return message.id;
  }
}