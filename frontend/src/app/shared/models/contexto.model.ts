export interface Contexto {
  id?: number;
  nombre?: string;
  descripcion?: string;
  scope?: string;
  creadoPor?: number | null;
  origen?: string;
  promptSeed?: string | null;
  vigencia?: boolean;
  fechaCreacion?: string;
}

// Tipo para el JSON parseado de promptSeed
export interface PromptSeedData {
  edad?: number | string;
  severidad?: string;
  [key: string]: any; // Permite cualquier otra propiedad
}

// Helpers para trabajar con scope
export const scopeToString = (scope: string | undefined): string => {
  if (!scope) return '';
  return scope.toLowerCase();
};

export const scopeToTags = (scope: string | undefined): string[] => {
  if (!scope) return [];
  
  const scopeTags: { [key: string]: string[] } = {
    'evaluacion_inicial': ['Evaluación', 'Primera consulta'],
    'seguimiento': ['Seguimiento', 'Apoyo continuo'],
    'intervencion': ['Intervención', 'Apoyo intensivo'],
    'crisis': ['Crisis', 'Urgente'],
    'evaluacion_tardia': ['Diagnóstico tardío', 'Evaluación']
  };

  return scopeTags[scope.toLowerCase()] || [scope];
};

/**
 * Parsea el promptSeed JSON de manera segura
 */
export const parsePromptSeed = (promptSeed: string | null | undefined): PromptSeedData | null => {
  if (!promptSeed) return null;
  
  try {
    // Si ya es un objeto, devolverlo
    if (typeof promptSeed === 'object') {
      return promptSeed as PromptSeedData;
    }
    
    // Intentar parsear como JSON
    return JSON.parse(promptSeed);
  } catch (error) {
    console.warn('Error al parsear promptSeed:', error);
    return null;
  }
};

/**
 * Extrae la edad del promptSeed
 */
export const getEdadFromPromptSeed = (contexto: Contexto): string => {
  // Primero intentar extraer del nombre (como fallback)
  const nombre = contexto.nombre || '';
  const matchNombre = nombre.match(/(\d+)\s*año/i);
  
  // Intentar del promptSeed
  const promptData = parsePromptSeed(contexto.promptSeed);
  if (promptData?.edad) {
    const edad = typeof promptData.edad === 'number' 
      ? promptData.edad 
      : parseInt(promptData.edad.toString());
    
    return isNaN(edad) ? (matchNombre ? `${matchNombre[1]} años` : 'N/A') : `${edad} años`;
  }
  
  // Fallback al nombre
  return matchNombre ? `${matchNombre[1]} años` : 'N/A';
};

/**
 * Genera tags dinámicamente desde el promptSeed
 * Excluye la edad y formatea los valores de manera legible
 */
export const getTagsFromPromptSeed = (contexto: Contexto): string[] => {
  const tags: string[] = [];
  
  // Parsear promptSeed
  const promptData = parsePromptSeed(contexto.promptSeed);
  if (!promptData) return tags;
  
  // Mapeo de keys a labels más legibles
  const keyLabels: { [key: string]: string } = {
    'severidad': 'Severidad',
    'comorbilidad': 'Comorbilidad',
    'contexto': 'Contexto',
    'tipo': 'Tipo',
    'foco': 'Foco',
    'nivel': 'Nivel',
    'area': 'Área',
    'caracteristica': 'Característica',
    'situacion': 'Situación',
    'perfil': 'Perfil'
  };
  
  // Iterar sobre todas las propiedades del JSON
  Object.keys(promptData).forEach(key => {
    // Excluir 'edad' y valores nulos/undefined
    if (key.toLowerCase() === 'edad' || !promptData[key]) return;
    
    const value = promptData[key];
    const label = keyLabels[key.toLowerCase()] || capitalize(key);
    
    // Formatear el tag
    if (typeof value === 'string' || typeof value === 'number') {
      tags.push(`${label}: ${capitalize(value.toString())}`);
    } else if (Array.isArray(value)) {
      // Si es un array, unir los valores
      tags.push(`${label}: ${value.map(v => capitalize(v.toString())).join(', ')}`);
    } else if (typeof value === 'boolean') {
      tags.push(value ? label : `Sin ${label}`);
    }
  });
  
  return tags;
};

/**
 * Capitaliza la primera letra de un string
 */
const capitalize = (str: string): string => {
  if (!str) return '';
  return str.charAt(0).toUpperCase() + str.slice(1).toLowerCase();
};