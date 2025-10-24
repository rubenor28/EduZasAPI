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
   - [x] NewTagDTO
   - [x] TagCriteriaDTO
   - [x] Tag Creator
   - [x] Tag Querier
   - [x] TagMapper
   - [ ] Registrar DI
2. [ ] Repositorio relacion etiquetas
   - [x] ContactTagDomain
     - DTO para ID
   - [x] ContactTag Creator
     - usa DTO para ID como entrada
   - [x] ContactTag Deleter
   - [x] ContactTag Reader
   - [x] ContactTagMapper
   - [x] Registrar DI
   - [x] Tests operaciones
3. [ ] Repositorio contacto
   - [x] NewContactDTO
   - [x] UpdateContactDTO
   - [x] ContactCriteriaDTO
   - [x] ContactCreator
   - [x] ContactUpdater
   - [x] ContactQuerier
   - [x] ContactReader
   - [ ] ContactMapper
   - [ ] Registrar DI
   - [ ] Tests operaciones
4. [ ] Caso de uso agregar etiqueta
   - Buscar si existe una etiqueta con ese texto
   - Si no existe, crear, si existe devolver ID
   - [ ] Test
5. [ ] Caso de uso eliminar una etiqueta de contacto
   - Eliminar la relacion etiqueta - contacto
   - [ ] Test
6. [ ] Caso de uso agregar contacto
   - Modificar campos contacto y recibir array de etiquetas que tiene
   - [ ] Test

## [ ] Evaluaciones

1. [ ] Repositorio para guardar tests
2. [ ] Repositorio para relacionar clases y tests
3. [ ] Caso de uso agregar tests.
   - Validar existencia de clase
   - Validar que el profesor sea parte de la clase (dueño o no)
   - Crear notificación de clase
4. [ ] Validador para tests
5. [ ] Caso de uso modificar tests.
6. [ ] Caso de uso leer test
7. [ ] Caso de uso buscar test
8. [ ] Caso de uso cambiar visibilidad de test por clase
9. [ ] Caso de uso eliminar clase
   - Debe eliminar tambien las relaciones con otras clases

## [ ] Contenido académico

## A futuro

1. [ ] Por ahora se asume que el usuario que solicita inscribirse
       es el que agrega al usuario a una clase, pero seria adecuado
       agregar a la API la capacidad de profesores dueños de agregar
       directamente usuarios y a los administradores
2. [ ] Implementar alguna abstraccion Bulk Create para creaciones en
       gran cantidad de forma thread-safety
