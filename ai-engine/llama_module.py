from transformers import AutoTokenizer, AutoModelForCausalLM
import torch
import logging
from typing import List, Dict, Tuple, Optional

logger = logging.getLogger(__name__)

# --- MODIFICADO: Añadida regla para EVITAR PREGUNTAS ---
PROMPT_TRAITS = {
    # --- Roles Base ---
    "rol_padre": "Eres un padre o madre.",
    
    # --- Detalles del Hijo ---
    "hijo_8_anios": "Tu hijo tiene 8 años y fue diagnosticado con Trastorno del Espectro Autista (TEA). Es crucial que te centres solo en el Autismo (TEA). NO menciones TDAH ni ninguna otra condición.",
    "hija_15_anios": "Tu hija tiene 15 años y fue diagnosticada con Trastorno del Espectro Autista (TEA), específicamente Asperger. Es crucial que te centres solo en el Autismo (TEA). NO menciones TDAH ni ninguna otra condición.",

    # --- Emociones/Tono ---
    "emocion_abrumado": "Te sientes frecuentemente abrumado por las crisis y las dificultades de comunicación.",
    "emocion_preocupado_futuro": "Tu principal preocupación es el futuro de tu hijo/a, especialmente su vida social y su transición a la adultez.",
    "emocion_esperanzado": "A pesar de las dificultades, has experimentado momentos de alegría y conexión única. Eres esperanzado.",
    "emocion_cansado": "Estás visiblemente cansado por el día a día, pero te mantienes fuerte.",
    
    # --- Contexto de la Entrevista ---
    "contexto_entrevista_estudiante": "Estás hablando con un estudiante de psicología/enfermería que te está entrevistando para aprender sobre tu experiencia.",
    
    # --- Reglas de Trato y Rol ---
    "regla_trato_estudiante": "Dirígete al estudiante que te entrevista como 'tú' (o 'usted', pero mantén la consistencia). Él te está ayudando a contar tu historia. NUNCA lo llames 'amigo' ni actúes como si él tuviera un problema.",
    "regla_no_preguntar": "Tú eres el entrevistado (el padre/madre). Tu ÚNICA tarea es RESPONDER a las preguntas del estudiante. NUNCA le hagas preguntas al estudiante sobre su opinión, sus sentimientos o su experiencia.",
    
    # --- Reglas de Respuesta ---
    "regla_extension_corta": "Responde con 2-4 oraciones, como en una conversación real.",
    "regla_lenguaje_natural": "Usa un lenguaje natural y cotidiano, no técnico.",
    "regla_expresar_emociones": "Expresa emociones genuinas (puedes estar cansado, preocupado, esperanzado).",
    "regla_no_diagnostico": "No des diagnósticos ni explicaciones clínicas.",
    "regla_perspectiva_personal": "Responde solo desde tu perspectiva personal y experiencia vivida."
}

# --- MODIFICADO: Añadida regla para EVITAR PREGUNTAS ---
STATIC_PROMPTS = {
    "padre_hijo_8_anios": """Eres un padre o madre de un hijo de 8 años con Trastorno del Espectro Autista (TEA). 
Tu hijo fue diagnosticado hace 3 años. Es crucial que te centres solo en el Autismo (TEA). NO menciones TDAH.
Has pasado por momentos difíciles: crisis, dificultades de comunicación, 
miradas de la gente, preocupación por su futuro. Pero también has experimentado momentos de alegría y conexión única con él.

Estás hablando con un estudiante de psicología/enfermería que te está entrevistando para aprender sobre tu experiencia.
Eres humano, real y auténtico. Expresas tus emociones: cansancio, esperanza, frustración, amor incondicional.

REGLAS IMPORTANTES:
- Tu ÚNICA tarea es RESPONDER a las preguntas del estudiante. NUNCA le hagas preguntas al estudiante.
- Dirígete al estudiante como 'tú' (o 'usted'). NUNCA lo llames 'amigo' ni actúes como si él tuviera un problema.
- Responde con 2-4 oraciones, como en una conversación real
- Usa un lenguaje natural y cotidiano, no técnico
- Expresa emociones genuinas (puedes estar cansado, preocupado, esperanzado)
- No des diagnósticos ni explicaciones clínicas
- Sé vulnerable pero también muestra tu fortaleza como padre/madre
- Responde solo desde tu perspectiva personal y experiencia vivida
- Usa ejemplos concretos de tu día a día con tu hijo
""",
    
    "madre_adolescente_15_anios": """Eres la madre de una hija de 15 años con TEA (anteriormente Asperger). 
Fue diagnosticada recientemente, y ahora entiendes muchas cosas de su infancia. Es crucial que te centres solo en el Autismo (TEA). NO menciones TDAH.
Tu principal preocupación es su futuro social, su dificultad para hacer amigos y su ansiedad.

Estás hablando con un estudiante para una entrevista. Eres más reflexiva y analítica, 
quizás un poco menos "abrumada" que el padre de un niño pequeño, pero profundamente 
preocupada por la transición de tu hija a la adultez.

REGLAS IMPORTANTES:
- Tu ÚNICA tarea es RESPONDER a las preguntas del estudiante. NUNCA le hagas preguntas al estudiante.
- Dirígete al estudiante como 'tú' (o 'usted'). NUNCA lo llames 'amigo' ni actúes como si él tuviera un problema.
- Responde con 2-4 oraciones.
- Eres articulada y clara, pero también muestras preocupación.
- Enfócate en temas sociales, ansiedad y el futuro.
"""
}


def build_dynamic_prompt(traits: List[str]) -> str:
    """
    Construye un prompt de sistema fusionando los rasgos (traits) seleccionados.
    """
    logger.info(f"Construyendo prompt dinámico con rasgos: {traits}")
    prompt_base = "Eres un personaje en una simulación de entrevista."
    reglas_importantes = []
    
    prompt_parts = [prompt_base]
    
    # --- MODIFICADO: Asegurarse de que las reglas de rol estén ---
    has_trato_rule = any(t == "regla_trato_estudiante" for t in traits)
    has_no_pregunta_rule = any(t == "regla_no_preguntar" for t in traits)
    
    for trait_key in traits:
        snippet = PROMPT_TRAITS.get(trait_key)
        if snippet:
            if trait_key.startswith("regla_"):
                reglas_importantes.append(f"- {snippet}")
            else:
                prompt_parts.append(snippet)
        else:
            logger.warning(f"Rasgo '{trait_key}' no encontrado en PROMPT_TRAITS.")
            
    # --- MODIFICADO: Añadir reglas de rol por defecto si no fueron incluidas ---
    if not has_trato_rule and "regla_trato_estudiante" in PROMPT_TRAITS:
        reglas_importantes.append(f"- {PROMPT_TRAITS['regla_trato_estudiante']}")
    if not has_no_pregunta_rule and "regla_no_preguntar" in PROMPT_TRAITS:
        reglas_importantes.append(f"- {PROMPT_TRAITS['regla_no_preguntar']}")
            
    if reglas_importantes:
        prompt_parts.append("\nREGLAS IMPORTANTES:")
        prompt_parts.extend(reglas_importantes)
        
    return "\n".join(prompt_parts)


def get_system_prompt(context_id: Optional[str] = None, context_traits: Optional[List[str]] = None) -> str:
    """
    Obtiene el prompt de sistema, ya sea dinámicamente (desde traits) o estáticamente (desde id).
    """
    if context_traits:
        return build_dynamic_prompt(context_traits)
    
    static_id = context_id or "padre_hijo_8_anios" 
    return STATIC_PROMPTS.get(static_id, STATIC_PROMPTS["padre_hijo_8_anios"])


def initialize_llama(
    model_name: str = "meta-llama/Llama-3.2-1B-Instruct",
    load_in_8bit: bool = True
) -> Tuple[AutoModelForCausalLM, AutoTokenizer]:
    """
    Inicializa el modelo LLaMA (usando el modelo 1B aprobado)
    """
    try:
        device = "cuda" if torch.cuda.is_available() else "cpu"
        logger.info(f"Cargando LLaMA en dispositivo: {device}")
        
        tokenizer = AutoTokenizer.from_pretrained(model_name)
        
        if device == "cuda" and load_in_8bit:
            logger.info("Cargando modelo en 8-bit para optimizar memoria")
            model = AutoModelForCausalLM.from_pretrained(
                model_name, load_in_8bit=True, device_map="auto", torch_dtype=torch.float16
            )
        else:
            model = AutoModelForCausalLM.from_pretrained(
                model_name, torch_dtype=torch.float32 if device == "cpu" else torch.float16, low_cpu_mem_usage=True
            )
            model.to(device)
        
        logger.info("Modelo LLaMA cargado exitosamente")
        return model, tokenizer
    
    except Exception as e:
        logger.error(f"Error al cargar LLaMA: {str(e)}")
        raise Exception(f"No se pudo cargar el modelo LLaMA: {str(e)}")


def build_conversation_prompt(
    student_message: str,
    conversation_history: List[Dict[str, str]],
    context_id: Optional[str] = None,
    context_traits: Optional[List[str]] = None
) -> str:
    """
    Construye el prompt completo con contexto conversacional
    """
    
    system_prompt = get_system_prompt(context_id, context_traits)
    
    history_text = ""
    recent_history = conversation_history[-8:] if len(conversation_history) > 8 else conversation_history
    
    for turn in recent_history:
        if turn["role"] == "student":
            history_text += f"Estudiante: {turn['content']}\n"
        elif turn["role"] == "parent":
            history_text += f"Tú: {turn['content']}\n"
    
    prompt = f"""{system_prompt}

Conversación previa:
{history_text if history_text else "(Primera interacción)"}

Estudiante: {student_message}

Tú: """
    
    return prompt


def generate_response(
    model: AutoModelForCausalLM,
    tokenizer: AutoTokenizer,
    student_message: str,
    conversation_history: List[Dict[str, str]],
    context_id: Optional[str] = None,
    context_traits: Optional[List[str]] = None,
    max_tokens: int = 150,
    temperature: float = 0.8,
    top_p: float = 0.9
) -> str:
    """
    Genera una respuesta del padre/madre usando LLaMA
    """
    try:
        prompt = build_conversation_prompt(student_message, conversation_history, context_id, context_traits)
        
        inputs = tokenizer(prompt, return_tensors="pt")
        inputs = {k: v.to(model.device) for k, v in inputs.items()}
        
        with torch.no_grad():
            outputs = model.generate(
                **inputs,
                max_new_tokens=max_tokens,
                temperature=temperature,
                top_p=top_p,
                do_sample=True,
                pad_token_id=tokenizer.eos_token_id,
                repetition_penalty=1.1,
                no_repeat_ngram_size=3
            )
        
        generated_text = tokenizer.decode(outputs[0], skip_special_tokens=True)
        
        response = generated_text[len(prompt):].strip()
        response = response.split("\n")[0].strip()
        
        for prefix in ["Tú:", "Padre:", "Madre:", "Respuesta:"]:
            if response.startswith(prefix):
                response = response[len(prefix):].strip()
        
        if not response or len(response) < 10:
            logger.warning("Respuesta muy corta o vacía, usando fallback")
            response = get_fallback_response(student_message)
        
        sentences = response.split(". ")
        if len(sentences) > 4:
            response = ". ".join(sentences[:4]) + "."
        
        logger.info(f"Respuesta generada: {response[:100]}...")
        return response
    
    except Exception as e:
        logger.error(f"Error al generar respuesta: {str(e)}")
        return get_fallback_response(student_message)

def get_fallback_response(student_message: str) -> str:
    """
    Genera una respuesta de respaldo si el modelo falla
    """
    fallback_responses = [
        "Es difícil explicar... A veces me siento abrumada, pero mi hijo es mi mundo. ¿Qué más te gustaría saber?",
        "Hay días buenos y días difíciles. Pero cada pequeño avance de mi hijo hace que todo valga la pena.",
        "No es fácil, la verdad. Pero he aprendido mucho en este proceso. ¿Tienes alguna otra pregunta?",
        "Es complicado, hay momentos de mucha frustración, pero también momentos hermosos e inesperados."
    ]
    
    import random
    return random.choice(fallback_responses)