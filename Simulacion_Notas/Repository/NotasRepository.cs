using Simulacion_Notas.Entities;

namespace Simulacion_Notas.Repository
{
    public static class NotaRepository
    {
        public static List<Nota> Notas = new()
{
    // --- Estudiante 1 ---
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Base de Datos", Grupo = "TI-102", NombreRubro = "Examen 1", NotaObtenida  = 80, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Base de Datos", Grupo = "TI-102", NombreRubro = "Examen 2", NotaObtenida  = 90, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Base de Datos", Grupo = "TI-102", NombreRubro = "Proyecto Final", NotaObtenida  = 95, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Programación II", Grupo = "TI-101", NombreRubro = "Examen Parcial", NotaObtenida  = 88, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Programación II", Grupo = "TI-101", NombreRubro = "Proyecto Final", NotaObtenida  = 93, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Redes y Comunicaciones", Grupo = "TI-202", NombreRubro = "Laboratorio", NotaObtenida  = 85, Año = 2024, Periodo = "PE04" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Redes y Comunicaciones", Grupo = "TI-202", NombreRubro = "Examen Teórico", NotaObtenida  = 78, Año = 2024, Periodo = "PE04" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "305620557", NombreEstudiante = "Gabriel Rodriguez Mora", Curso = "Administración de Proyectos", Grupo = "TI-303", NombreRubro = "Ensayo", NotaObtenida  = 87, Año = 2025, Periodo = "PE05" },

    // --- Estudiante 2 ---
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Base de Datos", Grupo = "ADM-102", NombreRubro = "Examen 1", NotaObtenida  = 70, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Base de Datos", Grupo = "ADM-102", NombreRubro = "Proyecto", NotaObtenida  = 85, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Administración", Grupo = "ADM-001", NombreRubro = "Ensayo", NotaObtenida  = 92, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Administración", Grupo = "ADM-001", NombreRubro = "Examen 1", NotaObtenida  = 81, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Contabilidad", Grupo = "ADM-002", NombreRubro = "Taller 1", NotaObtenida  = 89, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Contabilidad", Grupo = "ADM-002", NombreRubro = "Examen Final", NotaObtenida  = 90, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "208990123", NombreEstudiante = "Sofía Jiménez Salas", Curso = "Investigación Criminal", Grupo = "ADM-303", NombreRubro = "Proyecto", NotaObtenida  = 94, Año = 2026, Periodo = "PE05" },

    // --- Estudiante 3 ---
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Investigación Criminal", Grupo = "CR-303", NombreRubro = "Exposición", NotaObtenida  = 87, Año = 2024, Periodo = "PE02" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Investigación Criminal", Grupo = "CR-303", NombreRubro = "Informe", NotaObtenida  = 94, Año = 2024, Periodo = "PE02" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Investigación Criminal", Grupo = "CR-303", NombreRubro = "Examen Final", NotaObtenida  = 91, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Base de Datos", Grupo = "CR-201", NombreRubro = "Examen Teórico", NotaObtenida  = 78, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Base de Datos", Grupo = "CR-201", NombreRubro = "Práctica", NotaObtenida  = 82, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Contabilidad", Grupo = "CR-401", NombreRubro = "Laboratorio", NotaObtenida  = 88, Año = 2025, Periodo = "PE03" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509230789", NombreEstudiante = "Luis Fernández Castro", Curso = "Contabilidad", Grupo = "CR-401", NombreRubro = "Proyecto Final", NotaObtenida  = 92, Año = 2025, Periodo = "PE03" },

    // --- Estudiante 4 ---
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "605330888", NombreEstudiante = "Daniela Rojas Solano", Curso = "Programación I", Grupo = "TI-101", NombreRubro = "Examen 1", NotaObtenida  = 86, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "605330888", NombreEstudiante = "Daniela Rojas Solano", Curso = "Programación I", Grupo = "TI-101", NombreRubro = "Examen 2", NotaObtenida  = 88, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "605330888", NombreEstudiante = "Daniela Rojas Solano", Curso = "Programación I", Grupo = "TI-101", NombreRubro = "Proyecto Final", NotaObtenida  = 91, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "605330888", NombreEstudiante = "Daniela Rojas Solano", Curso = "Tecnologías de la Información", Grupo = "TI-401", NombreRubro = "Laboratorio", NotaObtenida  = 90, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "605330888", NombreEstudiante = "Daniela Rojas Solano", Curso = "Administración de Empresas", Grupo = "ADM-003", NombreRubro = "Ensayo", NotaObtenida  = 84, Año = 2025, Periodo = "PE05" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "605330888", NombreEstudiante = "Daniela Rojas Solano", Curso = "Administración de Empresas", Grupo = "ADM-003", NombreRubro = "Examen Final", NotaObtenida  = 86, Año = 2025, Periodo = "PE05" },

    // --- Estudiante 5 ---
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "402560334", NombreEstudiante = "Valeria Navarro Quesada", Curso = "Ingeniería de Software", Grupo = "TI-501", NombreRubro = "Examen Parcial", NotaObtenida = 89, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "402560334", NombreEstudiante = "Valeria Navarro Quesada", Curso = "Ingeniería de Software", Grupo = "TI-501", NombreRubro = "Proyecto Final", NotaObtenida = 94, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "402560334", NombreEstudiante = "Valeria Navarro Quesada", Curso = "Calidad de Software", Grupo = "TI-502", NombreRubro = "Informe", NotaObtenida = 90, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "402560334", NombreEstudiante = "Valeria Navarro Quesada", Curso = "Calidad de Software", Grupo = "TI-502", NombreRubro = "Examen Final", NotaObtenida = 92, Año = 2025, Periodo = "PE04" },

    // --- Estudiante 6 ---
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "308910442", NombreEstudiante = "Carlos Méndez Vargas", Curso = "Programación Web", Grupo = "TI-303", NombreRubro = "Examen 1", NotaObtenida = 77, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "308910442", NombreEstudiante = "Carlos Méndez Vargas", Curso = "Programación Web", Grupo = "TI-303", NombreRubro = "Proyecto Final", NotaObtenida = 85, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "308910442", NombreEstudiante = "Carlos Méndez Vargas", Curso = "Redes Avanzadas", Grupo = "TI-404", NombreRubro = "Laboratorio", NotaObtenida = 80, Año = 2024, Periodo = "PE02" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "308910442", NombreEstudiante = "Carlos Méndez Vargas", Curso = "Redes Avanzadas", Grupo = "TI-404", NombreRubro = "Examen Teórico", NotaObtenida = 83, Año = 2024, Periodo = "PE02" },

    // --- Estudiante 7 ---
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "209440221", NombreEstudiante = "Andrea Solís Chavarría", Curso = "Contabilidad General", Grupo = "ADM-204", NombreRubro = "Taller 1", NotaObtenida = 88, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "209440221", NombreEstudiante = "Andrea Solís Chavarría", Curso = "Contabilidad General", Grupo = "ADM-204", NombreRubro = "Examen Final", NotaObtenida = 91, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "209440221", NombreEstudiante = "Andrea Solís Chavarría", Curso = "Finanzas Empresariales", Grupo = "ADM-305", NombreRubro = "Ensayo", NotaObtenida = 85, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "209440221", NombreEstudiante = "Andrea Solís Chavarría", Curso = "Finanzas Empresariales", Grupo = "ADM-305", NombreRubro = "Proyecto", NotaObtenida = 89, Año = 2024, Periodo = "PE03" },

    // --- Estudiante 8 ---
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "P00123456", NombreEstudiante = "Michael Hernández López", Curso = "Inteligencia Artificial", Grupo = "TI-601", NombreRubro = "Examen 1", NotaObtenida = 90, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "P00123456", NombreEstudiante = "Michael Hernández López", Curso = "Inteligencia Artificial", Grupo = "TI-601", NombreRubro = "Proyecto Final", NotaObtenida = 96, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "P00123456", NombreEstudiante = "Michael Hernández López", Curso = "Aprendizaje Automático", Grupo = "TI-602", NombreRubro = "Investigación", NotaObtenida = 94, Año = 2025, Periodo = "PE05" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "P00123456", NombreEstudiante = "Michael Hernández López", Curso = "Aprendizaje Automático", Grupo = "TI-602", NombreRubro = "Examen Final", NotaObtenida = 91, Año = 2025, Periodo = "PE05" },

    // --- Estudiante 9 ---
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "309870551", NombreEstudiante = "Laura Campos Vargas", Curso = "Estadística", Grupo = "MAT-101", NombreRubro = "Examen 1", NotaObtenida = 75, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "309870551", NombreEstudiante = "Laura Campos Vargas", Curso = "Estadística", Grupo = "MAT-101", NombreRubro = "Examen 2", NotaObtenida = 83, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "309870551", NombreEstudiante = "Laura Campos Vargas", Curso = "Matemática Discreta", Grupo = "MAT-202", NombreRubro = "Proyecto", NotaObtenida = 88, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "309870551", NombreEstudiante = "Laura Campos Vargas", Curso = "Matemática Discreta", Grupo = "MAT-202", NombreRubro = "Examen Final", NotaObtenida = 91, Año = 2024, Periodo = "PE03" },

    // --- Estudiante 10 ---
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509661099", NombreEstudiante = "Ricardo Araya Brenes", Curso = "Ciberseguridad", Grupo = "TI-701", NombreRubro = "Examen 1", NotaObtenida = 82, Año = 2024, Periodo = "PE02" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509661099", NombreEstudiante = "Ricardo Araya Brenes", Curso = "Ciberseguridad", Grupo = "TI-701", NombreRubro = "Laboratorio", NotaObtenida = 86, Año = 2024, Periodo = "PE02" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509661099", NombreEstudiante = "Ricardo Araya Brenes", Curso = "Ética Profesional", Grupo = "TI-702", NombreRubro = "Ensayo", NotaObtenida = 90, Año = 2025, Periodo = "PE04" },
    new() { TipoIdentificacion = "PSP", NumeroIdentificacion = "509661099", NombreEstudiante = "Ricardo Araya Brenes", Curso = "Ética Profesional", Grupo = "TI-702", NombreRubro = "Exposición", NotaObtenida = 92, Año = 2025, Periodo = "PE04" },

    // --- Estudiante 11 ---
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "304441201", NombreEstudiante = "Mariana Vargas Esquivel", Curso = "Diseño de Interfaces", Grupo = "TI-302", NombreRubro = "Taller", NotaObtenida = 95, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "304441201", NombreEstudiante = "Mariana Vargas Esquivel", Curso = "Diseño de Interfaces", Grupo = "TI-302", NombreRubro = "Proyecto", NotaObtenida = 97, Año = 2023, Periodo = "PE01" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "304441201", NombreEstudiante = "Mariana Vargas Esquivel", Curso = "Diseño Web", Grupo = "TI-303", NombreRubro = "Examen", NotaObtenida = 90, Año = 2024, Periodo = "PE02" },
    new() { TipoIdentificacion = "CED", NumeroIdentificacion = "304441201", NombreEstudiante = "Mariana Vargas Esquivel", Curso = "Diseño Web", Grupo = "TI-303", NombreRubro = "Proyecto Final", NotaObtenida = 93, Año = 2024, Periodo = "PE02" },

    // --- Estudiante 12 ---
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "210012555", NombreEstudiante = "Javier Quesada León", Curso = "Arquitectura de Computadores", Grupo = "TI-203", NombreRubro = "Examen Teórico", NotaObtenida = 81, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "210012555", NombreEstudiante = "Javier Quesada León", Curso = "Arquitectura de Computadores", Grupo = "TI-203", NombreRubro = "Laboratorio", NotaObtenida = 88, Año = 2023, Periodo = "PE02" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "210012555", NombreEstudiante = "Javier Quesada León", Curso = "Sistemas Operativos", Grupo = "TI-204", NombreRubro = "Proyecto Final", NotaObtenida = 90, Año = 2024, Periodo = "PE03" },
    new() { TipoIdentificacion = "DMX", NumeroIdentificacion = "210012555", NombreEstudiante = "Javier Quesada León", Curso = "Sistemas Operativos", Grupo = "TI-204", NombreRubro = "Examen 1", NotaObtenida = 84, Año = 2024, Periodo = "PE03" }

};

    }
}
