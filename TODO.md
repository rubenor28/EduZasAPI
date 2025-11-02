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

1. [ ] Repositorio etiquetas
   - [ ] NewTagDTO
   - [ ] TagCriteriaDTO
   - [ ] Tag Creator
   - [ ] Tag Querier
   - [ ] TagMapper
   - [ ] Registrar DI
2. [ ] Repositorio relacion etiquetas
   - [ ] ContactTagDomain
     - DTO para ID
   - [ ] ContactTag Creator
     - usa DTO para ID como entrada
   - [ ] ContactTag Deleter
   - [ ] ContactTag Reader
   - [ ] ContactTagMapper
   - [ ] Registrar DI
   - [ ] Tests operaciones
3. [ ] Repositorio contacto
   - [ ] NewContactDTO
   - [ ] UpdateContactDTO
   - [ ] ContactCriteriaDTO
   - [ ] ContactCreator
   - [ ] ContactUpdater
   - [ ] ContactQuerier
   - [ ] ContactReader
   - [ ] ContactMapper
   - [ ] Registrar DI
   - [ ] Tests operaciones
4. [ ] Caso de uso agregar etiqueta

- [ ] Ruta agregar contacto
- [ ] Ruta buscar contacto
  - [ ] Buscar mis contactos
  - [ ] Buscar contactos registrados si eres admin
- [ ] Ruta eliminar contacto
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
2. [x] Test DAOs Tests
3. [ ] Validador de formato nuevo contacto
   - [ ] Registro en DI
   - [ ] Test
4. [ ] Validador de formato actualizar contacto
   - [ ] Test
5. [x] Caso de uso agregar test
   - [ ] Registro DI
   - [ ] Test
6. [x] Caso de uso modificar test
   - [ ] Registro DI
   - [ ] Test
7. [x] Caso de uso obtener test
   - [ ] Registro DI
   - [ ] Test
8. [x] Caso de uso buscar test
   - [ ] Registro DI
   - [ ] Test
9. [x] Caso de uso eliminar test
   - [ ] Registro DI
   - [ ] ELIMINAR LAS RESPUESTAS POSTERIORMENTE
   - [ ] ELIMINAR RELACIONES test per class
   - [ ] Test

10. [x] Crear relacion class - test
    - [ ] Test
11. [x] Eliminar relacion class - test
    - [ ] Test
12. [x] Actualizar relacion class - test
    - [ ] Test
13. [x] Mapper relacion clase - test
    - [x] Mapper nueva entidad - EF
    - [x] Mapper actualizar entidad - EF
    - [x] Mapper EF - dominio
14. [x] Caso de uso agregar tests a clase.
    - [ ] Test
15. [x] Caso de uso actualizar test por clase
    - [ ] Test
16. [x] Caso de uso eliminar test en clases
    - [ ] Eliminar respuestas asociadas a este test
    - [ ] Test

## [ ] Contenido académico

## A futuro

1. [ ] Por ahora se asume que el usuario que solicita inscribirse
       es el que agrega al usuario a una clase, pero seria adecuado
       agregar a la API la capacidad de profesores dueños de agregar
       directamente usuarios y a los administradores
2. [ ] Implementar alguna abstraccion Bulk Create para creaciones en
       gran cantidad de forma thread-safety
