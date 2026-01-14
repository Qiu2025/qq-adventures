# 🎮 Fundamentos de Videojuegos – Grupo 02

**Integrantes:**
- Zhiwei Zhang (220440)  - [@javizhangg](https://github.com/javizhangg)
- Xiaolei Zhu (220182)  - [@MiiNeLoC0](https://github.com/MiiNeLoC0)
- Jiade Zheng (230264)  - [@jiade-git](https://github.com/jiade-git)
- Yixiao Yao (230041)  - [@MASKYX](https://github.com/MASKYX)
- Siyuan Qiu (230260)  - [@Qiu2025](https://github.com/Qiu2025)
- Shuhang Pan (230257)  - [@usshng](https://github.com/usshng)

> ℹ️ **Estado del Proyecto (Vertical Slice):**
> Debido al marco temporal de la asignatura, el desarrollo actual se centra exclusivamente en el **bioma del desierto**. Esta versión representa una **Vertical Slice** funcional que demuestra las mecánicas, el arte y el ciclo de juego completo en este primer entorno, sirviendo como demostración de la visión final del proyecto.

---

## 📖 1. GDD

### 1.1 Descripción principal del videojuego y objetivos
Se trata de un **juego de plataformas en 2D** que narra la historia de un pingüino arrastrado por la marea hasta un desierto. Controlado por el jugador, deberá recorrer diferentes niveles llenos de obstáculos y con estilos artísticos variados para poder regresar a su hogar.

🎯 **Objetivo:** ayudar al pingüino a superar desafíos hasta llegar a su iglú.

---

### 1.2 Narrativa y personajes
El juego cuenta la historia de un pequeño pingüino que se encuentra plácidamente dormido boca arriba en el mar y que, de pronto, la marea lo arrastra lejos de su hogar y lo deja varado en medio de un árido desierto. Al despertar, descubre que se ha perdido, por lo que decide emprender un viaje lleno de peligros y retos para regresar al frío del Ártico.

Durante su travesía, el pingüino tendrá que enfrentarse a diferentes entornos, enemigos y trampas, superando niveles que introducirán retoques a la mecánica principal del disparo y pondrán a prueba el ingenio y los reflejos del jugador.

---

### 1.3 Jugadores objetivo
El juego está dirigido a **jugadores amantes del género de plataformas 2D** con dificultad **media-alta**. Contará con mecánicas que los jugadores deberán dominar para completar los distintos niveles del videojuego.

---

### 1.4 Mecánicas del juego  
#### 1.4.1 Mecánica principal de juego
La mecánica principal es el **movimiento**.  
Siendo un juego de plataformas, se prioriza que el movimiento sea fluido y divertido para el jugador objetivo.

Incluye:
- **Movimiento horizontal y vertical**  
- **Salto y caída**  
- **Doble salto**  
- **Dash** (movimiento rápido en una dirección)

#### 1.4.2 Mecánica complementaria 
La mecánica complementaria a la del movimiento es el uso de powerups. Estos powerups le dan al jugador la posibilidad de hacer un dash más en el aire, hacer un salto más en el aire, etc. 

---

### 1.5 Storyboard
🎬 **Cinemática inicial basada en el storyboard:** [Ver video en Google Drive](https://drive.google.com/file/d/1Te-6krGhnXz3SqRmLgXNlxqP2L8sbe1r/view?usp=sharing)

---

### 1.6 Género y referencias
**Género:** Plataformas 2D

**Referencias e inspiración:**
- *Bread and Fred* → estilo artístico del pingüino y los niveles  
- *Super Mario Bros 1* → movimiento clásico y fluidez  

---

### 1.7 Organización del equipo (Cuarta iteración)

| Miembro | Rol / Tareas | Descripción |
|----------|---------------|--------------|
| **Zhiwei Zhang** | Programador | Encargado de mejorar el diseño de la escena del selector de niveles y actualización de documentación en github. |
| **Xiaolei Zhu** | Programador | Encargado de meter en el menú principal, un menú con botones funcionales. Además de mejorar y añadir los apartados de opciones, sonido, controles y accesibilidad en el menú de pausa. |
| **Siyuan Qiu** | Programador | Encargado de crear nuevas animaciones (animación para la transición del nivel, animación de muerte), mejora del Ghost Replay, rediseño del HUD, implementación de algunos audios y efectos y del rediseño de los menús (principal y de pausa).  |
| **Yixiao Yao** | Programador QA (User Testing) | Encargado del C# testing, de la creación del efecto del viendo en los niveles, creación de animaciones, barra de cooldown para indicar el dash y otros efectos como el polvo al caer el suelo y al saltar. |
| **Jiade Zheng** | Programador QA (User Testing) | Encargado de integrar las medidas de accesibilidad (omitir mecánicas y recordatorios contextuales) y de realizar la auditoria de accesibilidad. |
| **Shuhang Pan** | Programador | Encargado de realizar la escena final en la que se enseña las estadísticas y créditos y creación de las distintas imágenes del pingüino (imagen promocional para el tráiler e imagen utilizada para el selector del nivel), además de la implementación de algunos audios y efectos. |

---

## 🧩 Enlaces adicionales
- 🎥 [Cinemática inicial – Storyboard](https://drive.google.com/file/d/1Te-6krGhnXz3SqRmLgXNlxqP2L8sbe1r/view?usp=sharing)
- 🎥 [Trailer](https://drive.google.com/file/d/13V9GcsxHoUSPRpR34ZCCYki4HJySSszg/view?usp=sharing)
- 💻 [Repositorio del proyecto en GitHub](https://github.com/Qiu2025/FdV)
- 🕹️ [Versión jugable en Unity Play - Iteración 1](https://play.unity.com/en/games/06fc4a0b-86ae-4d1c-b603-fb00b2041d86/fdv-iteracion-1)
- 🕹️ [Versión jugable en Unity Play - Iteración 2 Main](https://play.unity.com/en/games/709a73b2-393d-4be9-a65f-b6208a8f6214/fdv-iteracion-2-main)
- 🕹️ [Versión jugable en Unity Play - Iteración 2 Pistola de agua](https://play.unity.com/en/games/faf8b156-aa96-4577-bdd4-7e7252b4397b/fdv-iteracion-2-branch-pistola)
- 🕹️ [Versión jugable en Unity Play - Iteración 3](https://play.unity.com/en/games/bbaeee3d-c3a0-42b5-ae20-8f8a27410c10/fdv-iteracion-3)
- 🕹️ [Versión jugable en Unity Play - Iteración 4](https://play.unity.com/en/games/61f7ce15-57b0-4d72-8a5c-8a8d63848fd1/fdv-iteracion-4)

---

## 🧠 Licencia
Este proyecto está licenciado bajo la **Apache 2.0**.  
Consulta el archivo [`LICENSE`](./LICENSE) para más detalles.

---

## 📚 Cómo citar este proyecto
Si utilizas este videojuego o su código, por favor cita el proyecto como se indica en el archivo [`CITATION.cff`](./CITATION.cff).
