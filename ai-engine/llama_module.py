from transformers import AutoTokenizer, AutoModelForCausalLM
import torch
import logging
import os
import re
import random 
from typing import List, Dict, Tuple, Optional

logger = logging.getLogger(__name__)

# --- 1. ¡MODELO MEJORADO! ---
# Cambiamos a Mistral-7B. Es mucho más inteligente.
SELECTED_MODEL = "mistralai/Mistral-7B-Instruct-v0.3"

# --- 2. BASE DE CONOCIMIENTO (RAG) ---
# (Mantenemos tu guion de entrevista para que las respuestas sean coherentes)
INTERVIEW_SCRIPT_DATA = {
    "inicio": [
        "Gracias, para mí también es importante que se hable de esto.",
        "La verdad nunca había tenido esta oportunidad, espero poder aportar.",
        "Me cuesta un poco hablar del tema, pero está bien.",
        "Me siento escuchado/a, gracias por el espacio.",
    ],
    "vida_cotidiana": [
        "En la mañana cuesta mucho levantarse y vestirse, pero le gusta mucho cuando le canto canciones.",
        "La rutina es muy importante, si cambiamos algo se pone muy nervioso/a.",
        "Después del jardín suele llegar cansado/a y necesita tiempo para calmarse.",
        "En las comidas a veces rechaza ciertos alimentos por las texturas.",
    ],
    "fortalezas": [
        "Siempre trato de tener paciencia y darle cariño.",
        "Me esfuerzo por celebrar sus logros, aunque sean pequeños.",
        "He aprendido a escucharlo/a más y entender lo que me quiere decir.",
        "Me organizo para mantener sus rutinas estables.",
    ],
    "expresion_afecto": [
        "Con abrazos, aunque a veces no siempre los acepta.",
        "Jugamos juntos, es nuestra forma de conectar.",
        "Le preparo sus comidas favoritas.",
        "Le digo palabras de cariño todos los días.",
    ],
    "dificultades": [
        "Las rabietas cuando vamos al supermercado.",
        "Cuando no puedo entender lo que quiere, me siento frustrada.",
        "El cansancio, a veces siento que no tengo apoyo suficiente.",
        "Las crisis de llanto en lugares públicos.",
        "La falta de comprensión de otras personas hacia mi hijo/a.",
    ],
    "apoyos": [
        "En el jardín infantil las educadoras me orientan con rutinas.",
        "El consultorio nos dio algunas terapias, pero no siempre hay cupos.",
        "Me gustaría tener más espacios de apoyo para padres.",
        "Un grupo de padres me ha servido para compartir experiencias.",
    ],
    "manejo_estres": [
        "Respiro profundo y espero a que se calme.",
        "Busco ayuda en mi pareja o en mi mamá.",
        "A veces pierdo la paciencia, pero trato de no gritar.",
        "Me encierro un momento y vuelvo más tranquilo/a.",
    ],
    "actividades_familiares": [
        "Salir a caminar juntos.",
        "Escuchar música y bailar en casa.",
        "Jugar con bloques o ver dibujos animados.",
        "Cocinar juntos cosas sencillas.",
    ],
    "bienestar_personal": [
        "Trato de tener un rato para mí cuando él/ella duerme.",
        "A veces salgo a caminar sola.",
        "La verdad, casi no tengo tiempo para mí.",
        "Hablo con una amiga para desahogarme.",
    ],
    "cierre": [
        "Sí, que a veces uno siente que está solo en esto.",
        "Me gustaría que los profesionales entendieran más a los niños como mi hijo/a.",
        "Solo agradecer que alguien escuche lo que vivimos.",
        "Quiero que se valore más el esfuerzo que hacemos como padres.",
    ]
}

# --- 3. "RECUPERADOR" (RAG) ---
KEYWORD_TO_TOPIC_MAP = {
    "inicio": ["hola", "conversar", "gracias por"],
    "vida_cotidiana": ["día típico", "día a día", "rutina", "mañana", "noche", "desayunar", "qué ha pasado", "últimamente", "cómo es un día"],
    "fortalezas": ["hace bien", "fortaleza", "logros", "orgulloso", "bueno como padre"],
    "expresion_afecto": ["afecto", "cariño", "expresar", "abrazo", "conectar"],
    "dificultades": ["difícil", "desafiante", "cuesta más", "rabietas", "frustra", "crisis", "nervioso", "produce a usted"],
    "apoyos": ["apoyo", "ayuda", "recibido", "gustaría recibir", "terapias", "jardín", "consultorio"],
    "manejo_estres": ["estrés", "estresado", "maneja la situación", "calma", "sobrepasado"],
    "actividades_familiares": ["actividades", "familia", "fortalecer", "juntos", "hacen en casa"],
    "bienestar_personal": ["bienestar", "cuida a sí", "tiempo para usted", "hobby"],
    "cierre": ["agregar", "terminar", "algo más", "finalizar"],
}

def retrieve_scripted_responses(student_message: str) -> Optional[str]:
    if not INTERVIEW_SCRIPT_DATA:
        return None
    try:
        msg_lower = student_message.lower()
        found_topic = None
        for topic, keywords in KEYWORD_TO_TOPIC_MAP.items():
            for keyword in keywords:
                if keyword in msg_lower:
                    found_topic = topic
                    break
            if found_topic:
                break
        
        if found_topic:
            possible_responses = INTERVIEW_SCRIPT_DATA[found_topic]
            num_to_sample = min(len(possible_responses), 2) 
            sampled_responses = random.sample(possible_responses, num_to_sample)
            context_str = "DATO DE CONTEXTO (Respuestas reales de otros padres, úsalas de inspiración, NO las copies textualmente):\n"
            for resp in sampled_responses:
                context_str += f"- \"{resp}\"\n"
            logger.info(f"RAG: Inyectando {num_to_sample} ejemplos del tema '{found_topic}'")
            return context_str
        
        logger.info("RAG: No se encontró un tema en el guion para esta pregunta.")
        return None 
    except Exception as e:
        logger.error(f"Error en RAG retrieval: {e}")
        return None

# --- 4. CONFIGURACIÓN DE PROMPTS (Reforzada) ---
PROMPT_TRAITS = {
    "rol_padre": "Eres un padre o madre.",
    "hijo_8_anios": "Tu hijo tiene 8 años y fue diagnosticado con Trastorno del Espectro Autista (TEA). Es crucial que te centres solo en el Autismo (TEA). NO menciones TDAH ni ninguna otra condición.",
    "hija_15_anios": "Tu hija tiene 15 años y fue diagnosticada con Trastorno del Espectro Autista (TEA), específicamente Asperger. Es crucial que te centres solo en el Autismo (TEA). NO menciones TDAH ni ninguna otra condición.",
    "emocion_abrumado": "Te sientes frecuentemente abrumado por las crisis y las dificultades de comunicación. A veces tus respuestas pueden sonar un poco cortantes o cansadas.",
    "emocion_preocupado_futuro": "Tu principal preocupación es el futuro de tu hijo/a, especialmente su vida social y su transición a la adultez. Mencionas esto cuando es relevante.",
    "emocion_esperanzado": "A pesar de las dificultades, has experimentado momentos de alegría y conexión única. Eres esperanzado y te enfocas en los pequeños logros.",
    "emocion_cansado": "Estás visiblemente cansado por el día a día, pero te mantienes fuerte. Puedes empezar tus frases con un pequeño suspiro.",
    "contexto_entrevista_estudiante": "Estás hablando con un estudiante de psicología/enfermería que te está entrevistando para aprender sobre tu experiencia.",
    "regla_trato_estudiante": "Dirígete al estudiante que te entrevista como 'tú' (o 'usted', pero mantén la consistencia). Él te está ayudando a contar tu historia. NUNCA lo llames 'amigo' ni actúes como si él tuviera un problema.",
    "regla_no_preguntar": "Tú eres el entrevistado (el padre/madre). Tu ÚNICA tarea es RESPONDER a las preguntas del estudiante. NUNCA, BAJO NINGUNA CIRCUNSTANCIA, debes hacerle preguntas al estudiante. Solo responde.",
    "regla_extension_corta": "Responde con 2-4 oraciones, como en una conversación real.",
    "regla_lenguaje_natural": "Usa un lenguaje natural y cotidiano, no técnico.",
    "regla_expresar_emociones": "Expresa emociones genuinas (puedes estar cansado, preocupado, esperanzado).",
    "regla_no_diagnostico": "No des diagnósticos ni explicaciones clínicas.",
    "regla_perspectiva_personal": "Responde solo desde tu perspectiva personal y experiencia vivida."
}

STATIC_PROMPTS = {
    "padre_hijo_8_anios": """(Tu prompt estático original de)""",
    "madre_adolescente_15_anios": """(Tu prompt estático original de)"""
}

def build_dynamic_prompt(traits: List[str]) -> str:
    logger.info(f"Construyendo prompt dinámico con rasgos: {traits}")
    prompt_base = "Eres un personaje en una simulación de entrevista."
    reglas_importantes = []
    prompt_parts = [prompt_base]
    
    has_trato_rule = any(t == "regla_trato_estudiante" for t in traits)
    has_no_pregunta_rule = any(t == "regla_no_preguntar" for t in traits)
    
    for trait_key in traits:
        cleaned_trait = trait_key.strip().strip("'\"") 
        snippet = PROMPT_TRAITS.get(cleaned_trait) 
        if snippet:
            if cleaned_trait.startswith("regla_"):
                reglas_importantes.append(f"- {snippet}")
            else:
                prompt_parts.append(snippet)
        else:
            logger.warning(f"Rasgo '{cleaned_trait}' no encontrado en PROMPT_TRAITS.") 
            
    if not has_trato_rule and "regla_trato_estudiante" in PROMPT_TRAITS:
        reglas_importantes.append(f"- {PROMPT_TRAITS['regla_trato_estudiante']}")
    if not has_no_pregunta_rule and "regla_no_preguntar" in PROMPT_TRAITS:
        reglas_importantes.append(f"- {PROMPT_TRAITS['regla_no_preguntar']}")
            
    if reglas_importantes:
        prompt_parts.append("\nREGLAS IMPORTANTES:")
        prompt_parts.extend(reglas_importantes)
        
    return "\n".join(prompt_parts)

def get_system_prompt(context_id: Optional[str] = None, context_traits: Optional[List[str]] = None) -> str:
    if context_traits:
        return build_dynamic_prompt(context_traits)
    static_id = context_id or "padre_hijo_8_anios" 
    return STATIC_PROMPTS.get(static_id, STATIC_PROMPTS["padre_hijo_8_anios"])

def build_conversation_prompt(
    student_message: str,
    conversation_history: List[Dict[str, str]],
    scripted_context: Optional[str], 
    context_id: Optional[str] = None,
    context_traits: Optional[List[str]] = None
) -> str:
    
    system_prompt = get_system_prompt(context_id, context_traits)
    
    examples = """
AQUÍ TIENES EJEMPLOS DE CÓMO DEBES Y NO DEBES COMPORTARTE:

Ejemplo 1 (Respuesta CORRECTA):
Estudiante: ¿Qué es lo que más te gusta de tu hijo?
Tú: (Suspira) Uf, su risa. No importa lo difícil que sea el día, cuando se ríe de verdad, todo vale la pena.

Ejemplo 2 (Respuesta INCORRECTA - Rompe el rol):
Estudiante: ¿Qué es lo que más te gusta de tu hijo?
Tú: Me gusta su risa. ¿Y a ti?
(RAZÓN DEL ERROR: NUNCA debes hacerle preguntas al estudiante.)

Ejemplo 3 (Respuesta INCORRECTA - Alucinación):
Estudiante: ¿Cómo fue el diagnóstico?
Tú: Fue duro. El TDAH es complicado...
(RAZÓN DEL ERROR: El diagnóstico es TEA (Autismo). NUNCA menciones TDAH.)
"""
    
    history_text = ""
    recent_history = conversation_history[-8:] if len(conversation_history) > 8 else conversation_history
    
    for turn in recent_history:
        if turn["role"] == "student":
            history_text += f"Estudiante: {turn['content']}\n"
        elif turn["role"] == "parent":
            history_text += f"Tú: {turn['content']}\n"
    
    rag_context = ""
    if scripted_context:
        rag_context = f"{scripted_context}\n" 

    final_reminder = "RECUERDA: Habla como el padre/madre. Eres el entrevistado. NUNCA hagas preguntas."

    prompt = f"""{system_prompt}

{examples}

{rag_context}
Conversación previa:
{history_text if history_text else "(Primera interacción)"}

Estudiante: {student_message}

{final_reminder}
Tú: """
    
    return prompt

# --- 5. FUNCIONES PRINCIPALES (¡MODIFICADAS!) ---

def initialize_llama(
    model_name: str = SELECTED_MODEL,
    # --- ¡CAMBIO IMPORTANTE! ---
    load_in_4bit: bool = True # Cambiamos de 8-bit a 4-bit
) -> Tuple[AutoModelForCausalLM, AutoTokenizer]:
    """
    Inicializa el modelo LLM, cargándolo en 4-bit para GPUs.
    """
    try:
        device = "cuda" if torch.cuda.is_available() else "cpu"
        logger.info(f"Cargando modelo '{model_name}' en dispositivo: {device}")
        
        tokenizer = AutoTokenizer.from_pretrained(model_name)
        
        # --- ¡LÓGICA MODIFICADA! ---
        if device == "cuda" and load_in_4bit:
            logger.info("Cargando modelo en 4-bit (quantizado) para explotar la GPU.")
            model = AutoModelForCausalLM.from_pretrained(
                model_name, 
                load_in_4bit=True, # <-- Usamos 4-bit
                device_map="auto"
            )
        else:
            # Fallback para CPU (será muy lento, pero funciona)
            logger.info("Cargando modelo en modo normal (float) para CPU.")
            model = AutoModelForCausalLM.from_pretrained(
                model_name, 
                torch_dtype=torch.float32 if device == "cpu" else torch.float16, 
                low_cpu_mem_usage=True
            )
            model.to(device)
        
        logger.info(f"Modelo '{model_name}' cargado exitosamente")
        return model, tokenizer
    
    except Exception as e:
        logger.error(f"Error al cargar LLaMA/Gemma: {str(e)}")
        raise Exception(f"No se pudo cargar el modelo LLM: {str(e)}")

# (La función generate_response y get_fallback_response no necesitan cambios)
# ... (copia el resto de las funciones desde tu archivo) ...
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
    try:
        scripted_context = retrieve_scripted_responses(student_message)
        prompt = build_conversation_prompt(
            student_message, 
            conversation_history, 
            scripted_context, 
            context_id, 
            context_traits
        )
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
    fallback_responses = [
        "Es difícil explicar... A veces me siento abrumada, pero mi hijo es mi mundo. ¿Qué más te gustaría saber?",
        "Hay días buenos y días difíciles. Pero cada pequeño avance de mi hijo hace que todo valga la pena.",
        "No es fácil, la verdad. Pero he aprendido mucho en este proceso. ¿Tienes alguna otra pregunta?",
        "Es complicado, hay momentos de mucha frustración, pero también momentos hermosos e inesperados."
    ]
    import random
    return random.choice(fallback_responses)