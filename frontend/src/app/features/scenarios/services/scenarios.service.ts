import { Injectable } from '@angular/core';

export interface Scenario {
  id: number;
  title: string;
  edad: number;
  contexto: string;
  vistaPrevia: string;
  icono?: string;
  tags?: string[];
}

@Injectable({ 
  providedIn: 'root' 
})
export class ScenariosService {
  private scenarios: Scenario[] = [
    {
      id: 1,
      title: 'Niño de 4 años - Primeras señales',
      edad: 4,
      contexto: 'Padres preocupados por desarrollo social y comunicación',
      vistaPrevia: `Situación del caso:
• Padre: "Mi hijo no nos mira a los ojos cuando le hablamos"
• No responde cuando lo llaman por su nombre
• Prefiere jugar solo con sus carritos
• Se molesta cuando cambiamos su rutina diaria
• Repite palabras que escucha en la TV`,
      icono: 'child_care',
      tags: ['comunicación', 'social', 'rutinas']
    },
    {
      id: 2,
      title: 'Niña de 6 años - Diagnóstico reciente',
      edad: 6,
      contexto: 'Familia adaptándose al diagnóstico y buscando apoyo',
      vistaPrevia: `Situación del caso:
• Madre: "Nos acaban de confirmar que tiene autismo"
• Dificultades en el colegio con ruidos fuertes
• Muy inteligente pero evita juegos grupales
• Tiene obsesión con los dinosaurios
• Los padres no saben cómo explicárselo a la familia`,
      icono: 'school',
      tags: ['diagnóstico', 'educativo', 'familia']
    },
    {
      id: 3,
      title: 'Adolescente de 14 años - Transición',
      edad: 14,
      contexto: 'Desafíos de la adolescencia con autismo',
      vistaPrevia: `Situación del caso:
• Padre: "Desde que empezó la secundaria está muy ansioso"
• Cambios físicos de la pubertad lo angustian
• Dificultades para hacer amigos de su edad
• Excelente en matemáticas pero rechaza deportes
• Los padres temen por su futuro laboral`,
      icono: 'person',
      tags: ['adolescente', 'ansiedad', 'social']
    },
    {
      id: 4,
      title: 'Niño de 8 años - Comportamientos desafiantes',
      edad: 8,
      contexto: 'Crisis conductuales que afectan la dinámica familiar',
      vistaPrevia: `Situación del caso:
• Madre: "Últimamente tiene rabietas muy intensas"
• Se golpea la cabeza cuando está frustrado
• No tolera ciertos tejidos de ropa
• Hermanos menores tienen miedo de sus crisis
• Los padres se sienten agotados y sin recursos`,
      icono: 'warning',
      tags: ['conductual', 'sensorial', 'familia']
    },
    {
      id: 5,
      title: 'Niña de 10 años - Perfil femenino',
      edad: 10,
      contexto: 'Diagnóstico tardío en niña con enmascaramiento social',
      vistaPrevia: `Situación del caso:
• Madre: "Siempre pensé que era solo muy tímida"
• Imita comportamientos de sus compañeras
• Llega agotada del colegio, colapsa en casa
• Intereses intensos pero los oculta en público
• Autoestima baja, se compara constantemente`,
      icono: 'face',
      tags: ['femenino', 'masking', 'autoestima']
    }
  ];

  /**
   * Obtiene todos los escenarios disponibles
   */
  getScenarios(): Scenario[] {
    return [...this.scenarios];
  }

  /**
   * Obtiene un escenario aleatorio
   */
  getRandomScenario(): Scenario {
    const randomIndex = Math.floor(Math.random() * this.scenarios.length);
    return { ...this.scenarios[randomIndex] };
  }

  /**
   * Obtiene un escenario por ID
   */
  getScenarioById(id: number): Scenario | undefined {
    const scenario = this.scenarios.find(s => s.id === id);
    return scenario ? { ...scenario } : undefined;
  }

  /**
   * Filtra escenarios por edad
   */
  getScenariosByAge(minAge: number, maxAge: number): Scenario[] {
    return this.scenarios
      .filter(scenario => scenario.edad >= minAge && scenario.edad <= maxAge)
      .map(scenario => ({ ...scenario }));
  }

  /**
   * Obtiene estadísticas básicas
   */
  getScenarioStats(): {
    total: number;
    avgAge: number;
    ageRange: string;
  } {
    const ages = this.scenarios.map(s => s.edad);
    return {
      total: this.scenarios.length,
      avgAge: Math.round(ages.reduce((sum, age) => sum + age, 0) / ages.length),
      ageRange: `${Math.min(...ages)} - ${Math.max(...ages)} años`
    };
  }

  /**
   * Busca escenarios por término
   */
  searchScenarios(searchTerm: string): Scenario[] {
    const term = searchTerm.toLowerCase().trim();
    if (!term) return this.getScenarios();

    return this.scenarios.filter(scenario => 
      scenario.title.toLowerCase().includes(term) ||
      scenario.contexto.toLowerCase().includes(term) ||
      scenario.tags?.some(tag => tag.toLowerCase().includes(term))
    ).map(scenario => ({ ...scenario }));
  }
}