# Cosas por hacer API

## [ ] Gestión de usuarios

1. [ ] Validacion de emails (a considerar)
2. [ ] Recupearción de contraseñas

## Respaldo y restauración

1. [x] Crear interfaz de respaldador
2. [ ] Crear respaldador basado en ejecutar el binario mariadb-dump
3. [ ] Crear caso de uso respaldado
4. [ ] Crear endpoint para crear un respaldo

## [x] Notificaciones

1. Repositorio notificaciones
   - [x] NewNotificationDTO
   - [x] NotificationCriteriaDTO
     - [x] Agregar metodo para filtrar por clase y/o estudiante
2. Repositorio notificaciones por usuario
   - [x] UserNotification
   - [x] NewUserNotificationDTO
   - [x] UpdateUserNotificationDTO (Solo cambiaria visto o no)
     - [x] Filtrar por estudiante o clase
3. [x] Caso de uso creacion notificacion de clase
   - Crear notificacion general y asignar a usuarios de clase
4. [x] Caso de uso obtener notificacion por usuario
5. [x] Caso de uso marcar como notificacion leida
6. [x] Caso de uso eliminar notificaciones de usuario
   - Sólo para que usuarios puedan eliminar las notificaciones que ven
     no la notificacion como tal, posiblemente no permitir eliminar
     notificaciones generales

## [ ] Contactos

> ![NOTE] Deberia considerar que el DTO de contacto al menos el publico tiene inscrustadas las etiquetas en un array?
> ![NOTE] Diseñar nuevas clases de EF que solo agregan o eliminan

1. [x] Repositorio etiquetas
   - [x] NewTagDTO
   - [x] TagCriteriaDTO
   - [x] Tag Creator
   - [x] Tag Querier
   - [x] TagMapper
   - [x] Registrar DI
2. [x] Repositorio relacion etiquetas
   - [x] ContactTagDomain
     - DTO para ID
   - [x] ContactTag Creator
     - usa DTO para ID como entrada
   - [x] ContactTag Deleter
   - [x] ContactTag Reader
   - [x] ContactTagMapper
   - [x] Registrar DI
   - [x] Tests operaciones
3. [x] Repositorio contacto
   - [x] NewContactDTO
   - [x] UpdateContactDTO
   - [x] ContactCriteriaDTO
   - [x] ContactCreator
   - [x] ContactUpdater
   - [x] ContactQuerier
   - [x] ContactReader
   - [x] ContactMapper
   - [x] Registrar DI
   - [x] Tests operaciones
4. [x] Caso de uso agregar etiqueta
   - Buscar si existe una etiqueta con ese texto
   - [x] Test
5. [x] Caso de uso agregar una etiqueta a contacto
   - Crear relacion etiqueta - contacto
   - [x] Test
6. [x] Caso de uso eliminar una etiqueta de contacto
   - Eliminar la relacion etiqueta - contacto
   - [x] Test
7. [x] Caso de uso agregar contacto
   - Modificar campos contacto y recibir array de etiquetas que tiene
   - [x] Test

- [x] Ruta agregar contacto
- [x] Ruta buscar contacto
  - [x] Buscar mis contactos
  - [x] Buscar contactos registrados si eres admin
- [x] Ruta eliminar contacto
- [ ] Ruta modificar contacto
- [ ] Ruta agregar etiqueta a contacto
- [ ] Ruta eliminar etiqueta a contacto

## [ ] Evaluaciones

1. [ ] Repositorio para guardar tests
   - [x] Crear test
     - [x] Registro DI
   - [x] Modificar test
     - [x] Registro DI
   - [x] Eliminar test
     - [x] Registro DI
   - [x] Obtener test por Id
     - [x] Registro DI
   - [x] Buscar test
     - [x] Registro DI
   - [x] Mapper EF a dominio
     - [x] Registro DI
   - [x] Mapper nuevo a EF
     - [x] Registro DI
   - [x] Mapper update a EF
     - [x] Registro DI
1. [x] Test DAOs Tests
1. [ ] Validador de formato nuevo contacto
   - Test [ ]
1. [ ] Validador de formato actualizar contacto
   - [ ] Test
1. [x] Caso de uso agregar test
   - [ ] Registro DI
   - [ ] Test
1. [x] Caso de uso modificar test
   - [ ] Registro DI
   - [ ] Test
1. [x] Caso de uso obtener test
   - [ ] Registro DI
   - [ ] Test
1. [x] Caso de uso buscar test
   - [ ] Registro DI
   - [ ] Test
1. [x] Caso de uso eliminar test
   - [ ] Registro DI
   - [ ] ELIMINAR LAS RESPUESTAS POSTERIORMENTE
   - [ ] ELIMINAR RELACIONES test per class
   - [ ] Test

1. [x] Crear relacion class - test
1. [x] Eliminar relacion class - test
1. [x] Actualizar relacion class - test
1. [x] Mapper relacion clase - test
   - [x] Mapper nueva entidad - EF
   - [x] Mapper actualizar entidad - EF
   - [x] Mapper EF - dominio
1. [ ] Caso de uso agregar tests a clase.
1. [ ] Caso de uso cambiar visibilidad de test por clase
1. [ ] Caso de uso eliminar test en clases
   - [ ] Eliminar respuestas (?)

## [ ] Contenido académico

## A futuro

1. [ ] Por ahora se asume que el usuario que solicita inscribirse
       es el que agrega al usuario a una clase, pero seria adecuado
       agregar a la API la capacidad de profesores dueños de agregar
       directamente usuarios y a los administradores
2. [ ] Implementar alguna abstraccion Bulk Create para creaciones en
       gran cantidad de forma thread-safety
