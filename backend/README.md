# Neuropuentes backend
## estructura del backend

NeuroPuentes/  
│  
├── src/  
│   └── NeuroPuentes.API/  
│       ├── Controllers/         # Endpoints HTTP  
│       ├── Models/              # Modelos de base de datos  
│       ├── DTOs/                # Objetos de transferencia de datos  
│       ├── Services/            # Lógica de negocio   
│       ├── Repositories/        # Acceso a datos  
│       ├── Program.cs           # Configuración principal  
│       └── NeuroPuentes.API.csproj  
│  
├── tests/  
│   └── NeuroPuentes.Tests/  
│       ├── UnitTest1.cs         # Ejemplos de test  
│       └── NeuroPuentes.Tests.csproj  
│  
└── NeuroPuentes.sln             # Solución que agrupa API y tests  


## Como ejecutar los proyectos

* Arrancar API
  
```bash
  dotnet run --project src/NeuroPuentes.API/
```
Asi como tambien puedes ejectutar `dotnet run` en el directorio `./src/NeuroPuentes.API`

* Ejectutar tests
  
```bash
  dotnet test
```