CREATE TABLE users (
    user_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    email VARCHAR(30) NOT NULL,
    password VARCHAR(60) NOT NULL,
    first_name VARCHAR(20),
    mid_name VARCHAR(20),
    father_lastname VARCHAR(20),
    mother_lastname VARCHAR(20),
    role INT UNSIGNED NOT NULL DEFAULT 0,
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id),
    UNIQUE KEY uq_users_email (email)
);

CREATE TABLE classes (
    class_id CHAR(20) NOT NULL,
    class_name VARCHAR(100),
    section VARCHAR(100),
    subject VARCHAR(100),
    color VARCHAR(7) NOT NULL DEFAULT '#007bff',
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (class_id)
);

CREATE TABLE agenda_contacts (
    agenda_owner_id BIGINT UNSIGNED NOT NULL,
    contact_id BIGINT UNSIGNED NOT NULL,
    alias VARCHAR(40),
    notes TEXT,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (agenda_owner_id, contact_id),
    CONSTRAINT fk_agenda_contacts_owner
        FOREIGN KEY (agenda_owner_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_agenda_contacts_contact
        FOREIGN KEY (contact_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE tags (
    tag_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    text VARCHAR(30) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tag_id),
    UNIQUE KEY uq_tags_text (text)
);

CREATE TABLE contact_tags (
    tag_id BIGINT UNSIGNED NOT NULL,
    agenda_owner_id BIGINT UNSIGNED NOT NULL,
    contact_id BIGINT UNSIGNED NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (tag_id, agenda_owner_id, contact_id),
    CONSTRAINT fk_contact_tags_tag
        FOREIGN KEY (tag_id) REFERENCES tags(tag_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_contact_tags_agenda
        FOREIGN KEY (agenda_owner_id, contact_id)
        REFERENCES agenda_contacts(agenda_owner_id, contact_id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE class_professors (
    class_id CHAR(20) NOT NULL,
    professor_id BIGINT UNSIGNED NOT NULL,
    is_owner TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (class_id, professor_id),
    KEY idx_class_professors_professor (professor_id),
    CONSTRAINT fk_class_professors_class
        FOREIGN KEY (class_id) REFERENCES classes(class_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_class_professors_user
        FOREIGN KEY (professor_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE class_students (
    class_id CHAR(20) NOT NULL,
    student_id BIGINT UNSIGNED NOT NULL,
    hidden TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (class_id, student_id),
    KEY idx_class_students_student (student_id),
    CONSTRAINT fk_class_students_class
        FOREIGN KEY (class_id) REFERENCES classes(class_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_class_students_user
        FOREIGN KEY (student_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE resources (
    resource_id BINARY(16) NOT NULL,
    professor_id BIGINT UNSIGNED NOT NULL,
    title VARCHAR(60),
    content JSON,
    color VARCHAR(7) NOT NULL DEFAULT '#1976d2',
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (resource_id),
    KEY idx_resources_professor (professor_id),
    CONSTRAINT fk_resources_user
        FOREIGN KEY (professor_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE class_resources (
    class_id CHAR(20) NOT NULL,
    resource_id BINARY(16) NOT NULL,
    hidden TINYINT(1) NOT NULL DEFAULT 0,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (class_id, resource_id),
    KEY idx_class_resources_resource (resource_id),
    CONSTRAINT fk_class_resources_class
        FOREIGN KEY (class_id) REFERENCES classes(class_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_class_resources_resource
        FOREIGN KEY (resource_id) REFERENCES resources(resource_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE tests (
    test_id BINARY(16) NOT NULL,
    professor_id BIGINT UNSIGNED NOT NULL,
    title VARCHAR(60),
    content JSON,
    time_limit_minutes INT UNSIGNED,
    color VARCHAR(7) NOT NULL DEFAULT '#1976d2',
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (test_id),
    KEY idx_tests_professor (professor_id),
    CONSTRAINT fk_tests_user
        FOREIGN KEY (professor_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE tests_per_class (
    test_id BINARY(16) NOT NULL,
    class_id CHAR(20) NOT NULL,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (test_id, class_id),
    KEY idx_tests_per_class_class (class_id),
    CONSTRAINT fk_tests_per_class_test
        FOREIGN KEY (test_id) REFERENCES tests(test_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_tests_per_class_class
        FOREIGN KEY (class_id) REFERENCES classes(class_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE answer (
    test_id BINARY(16) NOT NULL,
    class_id CHAR(20) NOT NULL,
    user_id BIGINT UNSIGNED NOT NULL,
    content JSON,
    metadata JSON,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (test_id, class_id, user_id),
    CONSTRAINT fk_answer_user
        FOREIGN KEY (user_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_answer_test_class
        FOREIGN KEY (test_id, class_id)
        REFERENCES tests_per_class(test_id, class_id)
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE notifications (
    notification_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    class_id CHAR(20),
    title VARCHAR(100),
    active TINYINT(1) NOT NULL DEFAULT 1,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (notification_id),
    KEY idx_notifications_class (class_id),
    CONSTRAINT fk_notifications_class
        FOREIGN KEY (class_id) REFERENCES classes(class_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE notification_per_user (
    notification_id BIGINT UNSIGNED NOT NULL,
    user_id BIGINT UNSIGNED NOT NULL,
    readed TINYINT(1),
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (notification_id, user_id),
    KEY idx_notification_per_user_user (user_id),
    CONSTRAINT fk_notification_per_user_notification
        FOREIGN KEY (notification_id) REFERENCES notifications(notification_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_notification_per_user_user
        FOREIGN KEY (user_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE resource_view_sessions (
    id BINARY(16) NOT NULL,
    user_id BIGINT UNSIGNED NOT NULL,
    resource_id BINARY(16) NOT NULL,
    class_id CHAR(20),
    start_time_utc DATETIME,
    end_time_utc DATETIME,
    duration_seconds INT,
    created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    KEY idx_rvs_user (user_id),
    KEY idx_rvs_resource (resource_id),
    KEY idx_rvs_class (class_id),
    CONSTRAINT fk_rvs_user
        FOREIGN KEY (user_id) REFERENCES users(user_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_rvs_resource
        FOREIGN KEY (resource_id) REFERENCES resources(resource_id) 
        ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT fk_rvs_class
        FOREIGN KEY (class_id) REFERENCES classes(class_id) 
        ON DELETE CASCADE ON UPDATE CASCADE
);

-- TABLAS PARA GESTION DE TAREAS EN SEGUNDO PLANO QUARTZ
-- https://www.quartz-scheduler.net/documentation/quartz-3.x/db/

CREATE TABLE QRTZ_JOB_DETAILS(
SCHED_NAME VARCHAR(120) NOT NULL,
JOB_NAME VARCHAR(200) NOT NULL,
JOB_GROUP VARCHAR(200) NOT NULL,
DESCRIPTION VARCHAR(250) NULL,
JOB_CLASS_NAME VARCHAR(250) NOT NULL,
IS_DURABLE BOOLEAN NOT NULL,
IS_NONCONCURRENT BOOLEAN NOT NULL,
IS_UPDATE_DATA BOOLEAN NOT NULL,
REQUESTS_RECOVERY BOOLEAN NOT NULL,
JOB_DATA BLOB NULL,
PRIMARY KEY (SCHED_NAME,JOB_NAME,JOB_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_TRIGGERS (
SCHED_NAME VARCHAR(120) NOT NULL,
TRIGGER_NAME VARCHAR(200) NOT NULL,
TRIGGER_GROUP VARCHAR(200) NOT NULL,
JOB_NAME VARCHAR(200) NOT NULL,
JOB_GROUP VARCHAR(200) NOT NULL,
DESCRIPTION VARCHAR(250) NULL,
NEXT_FIRE_TIME BIGINT(19) NULL,
PREV_FIRE_TIME BIGINT(19) NULL,
PRIORITY INTEGER NULL,
TRIGGER_STATE VARCHAR(16) NOT NULL,
TRIGGER_TYPE VARCHAR(8) NOT NULL,
START_TIME BIGINT(19) NOT NULL,
END_TIME BIGINT(19) NULL,
CALENDAR_NAME VARCHAR(200) NULL,
MISFIRE_INSTR SMALLINT(2) NULL,
JOB_DATA BLOB NULL,
PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),
FOREIGN KEY (SCHED_NAME,JOB_NAME,JOB_GROUP)
REFERENCES QRTZ_JOB_DETAILS(SCHED_NAME,JOB_NAME,JOB_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_SIMPLE_TRIGGERS (
SCHED_NAME VARCHAR(120) NOT NULL,
TRIGGER_NAME VARCHAR(200) NOT NULL,
TRIGGER_GROUP VARCHAR(200) NOT NULL,
REPEAT_COUNT BIGINT(7) NOT NULL,
REPEAT_INTERVAL BIGINT(12) NOT NULL,
TIMES_TRIGGERED BIGINT(10) NOT NULL,
PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),
FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)
REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_CRON_TRIGGERS (
SCHED_NAME VARCHAR(120) NOT NULL,
TRIGGER_NAME VARCHAR(200) NOT NULL,
TRIGGER_GROUP VARCHAR(200) NOT NULL,
CRON_EXPRESSION VARCHAR(120) NOT NULL,
TIME_ZONE_ID VARCHAR(80),
PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),
FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)
REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_SIMPROP_TRIGGERS
  (          
    SCHED_NAME VARCHAR(120) NOT NULL,
    TRIGGER_NAME VARCHAR(200) NOT NULL,
    TRIGGER_GROUP VARCHAR(200) NOT NULL,
    STR_PROP_1 VARCHAR(512) NULL,
    STR_PROP_2 VARCHAR(512) NULL,
    STR_PROP_3 VARCHAR(512) NULL,
    INT_PROP_1 INT NULL,
    INT_PROP_2 INT NULL,
    LONG_PROP_1 BIGINT NULL,
    LONG_PROP_2 BIGINT NULL,
    DEC_PROP_1 NUMERIC(13,4) NULL,
    DEC_PROP_2 NUMERIC(13,4) NULL,
    BOOL_PROP_1 BOOLEAN NULL,
    BOOL_PROP_2 BOOLEAN NULL,
    TIME_ZONE_ID VARCHAR(80) NULL,
    PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),
    FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP) 
    REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_BLOB_TRIGGERS (
SCHED_NAME VARCHAR(120) NOT NULL,
TRIGGER_NAME VARCHAR(200) NOT NULL,
TRIGGER_GROUP VARCHAR(200) NOT NULL,
BLOB_DATA BLOB NULL,
PRIMARY KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP),
INDEX (SCHED_NAME,TRIGGER_NAME, TRIGGER_GROUP),
FOREIGN KEY (SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP)
REFERENCES QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_CALENDARS (
SCHED_NAME VARCHAR(120) NOT NULL,
CALENDAR_NAME VARCHAR(200) NOT NULL,
CALENDAR BLOB NOT NULL,
PRIMARY KEY (SCHED_NAME,CALENDAR_NAME))
ENGINE=InnoDB;

CREATE TABLE QRTZ_PAUSED_TRIGGER_GRPS (
SCHED_NAME VARCHAR(120) NOT NULL,
TRIGGER_GROUP VARCHAR(200) NOT NULL,
PRIMARY KEY (SCHED_NAME,TRIGGER_GROUP))
ENGINE=InnoDB;

CREATE TABLE QRTZ_FIRED_TRIGGERS (
SCHED_NAME VARCHAR(120) NOT NULL,
ENTRY_ID VARCHAR(140) NOT NULL,
TRIGGER_NAME VARCHAR(200) NOT NULL,
TRIGGER_GROUP VARCHAR(200) NOT NULL,
INSTANCE_NAME VARCHAR(200) NOT NULL,
FIRED_TIME BIGINT(19) NOT NULL,
SCHED_TIME BIGINT(19) NOT NULL,
PRIORITY INTEGER NOT NULL,
STATE VARCHAR(16) NOT NULL,
JOB_NAME VARCHAR(200) NULL,
JOB_GROUP VARCHAR(200) NULL,
IS_NONCONCURRENT BOOLEAN NULL,
REQUESTS_RECOVERY BOOLEAN NULL,
PRIMARY KEY (SCHED_NAME,ENTRY_ID))
ENGINE=InnoDB;

CREATE TABLE QRTZ_SCHEDULER_STATE (
SCHED_NAME VARCHAR(120) NOT NULL,
INSTANCE_NAME VARCHAR(200) NOT NULL,
LAST_CHECKIN_TIME BIGINT(19) NOT NULL,
CHECKIN_INTERVAL BIGINT(19) NOT NULL,
PRIMARY KEY (SCHED_NAME,INSTANCE_NAME))
ENGINE=InnoDB;

CREATE TABLE QRTZ_LOCKS (
SCHED_NAME VARCHAR(120) NOT NULL,
LOCK_NAME VARCHAR(40) NOT NULL,
PRIMARY KEY (SCHED_NAME,LOCK_NAME))
ENGINE=InnoDB;

CREATE INDEX IDX_QRTZ_J_REQ_RECOVERY ON QRTZ_JOB_DETAILS(SCHED_NAME,REQUESTS_RECOVERY);
CREATE INDEX IDX_QRTZ_J_GRP ON QRTZ_JOB_DETAILS(SCHED_NAME,JOB_GROUP);

CREATE INDEX IDX_QRTZ_T_J ON QRTZ_TRIGGERS(SCHED_NAME,JOB_NAME,JOB_GROUP);
CREATE INDEX IDX_QRTZ_T_JG ON QRTZ_TRIGGERS(SCHED_NAME,JOB_GROUP);
CREATE INDEX IDX_QRTZ_T_C ON QRTZ_TRIGGERS(SCHED_NAME,CALENDAR_NAME);
CREATE INDEX IDX_QRTZ_T_G ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_GROUP);
CREATE INDEX IDX_QRTZ_T_STATE ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_STATE);
CREATE INDEX IDX_QRTZ_T_N_STATE ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP,TRIGGER_STATE);
CREATE INDEX IDX_QRTZ_T_N_G_STATE ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_GROUP,TRIGGER_STATE);
CREATE INDEX IDX_QRTZ_T_NEXT_FIRE_TIME ON QRTZ_TRIGGERS(SCHED_NAME,NEXT_FIRE_TIME);
CREATE INDEX IDX_QRTZ_T_NFT_ST ON QRTZ_TRIGGERS(SCHED_NAME,TRIGGER_STATE,NEXT_FIRE_TIME);
CREATE INDEX IDX_QRTZ_T_NFT_MISFIRE ON QRTZ_TRIGGERS(SCHED_NAME,MISFIRE_INSTR,NEXT_FIRE_TIME);
CREATE INDEX IDX_QRTZ_T_NFT_ST_MISFIRE ON QRTZ_TRIGGERS(SCHED_NAME,MISFIRE_INSTR,NEXT_FIRE_TIME,TRIGGER_STATE);
CREATE INDEX IDX_QRTZ_T_NFT_ST_MISFIRE_GRP ON QRTZ_TRIGGERS(SCHED_NAME,MISFIRE_INSTR,NEXT_FIRE_TIME,TRIGGER_GROUP,TRIGGER_STATE);

CREATE INDEX IDX_QRTZ_FT_TRIG_INST_NAME ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,INSTANCE_NAME);
CREATE INDEX IDX_QRTZ_FT_INST_JOB_REQ_RCVRY ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,INSTANCE_NAME,REQUESTS_RECOVERY);
CREATE INDEX IDX_QRTZ_FT_J_G ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,JOB_NAME,JOB_GROUP);
CREATE INDEX IDX_QRTZ_FT_JG ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,JOB_GROUP);
CREATE INDEX IDX_QRTZ_FT_T_G ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,TRIGGER_NAME,TRIGGER_GROUP);
CREATE INDEX IDX_QRTZ_FT_TG ON QRTZ_FIRED_TRIGGERS(SCHED_NAME,TRIGGER_GROUP);

-- DATOS DE PRUEBA

-- ==========================================================
-- 1. TABLA: users
-- ==========================================================
INSERT INTO users (user_id, email, password, first_name, mid_name, father_lastname, mother_lastname, role, active, created_at, modified_at) VALUES
(1, 'admin@test.com', '$2a$11$aGNE8tRI3eWh0SpqS7HcduoZeb/vv8SVpn6.W16dKWi84z/6746Z6', 'ADMIN', NULL, 'ADMIN', NULL, 2, 1, '2025-12-11 16:45:40', '2025-12-15 18:53:51'),
(2, 'admin2@test.com', '$2a$11$x.3grmfmv2OhoYVklGv3DOxs2V/zSsHa6AGSu4MF2n8/pdU/F5IlO', 'ADMIN', 'DOS', 'TEST', NULL, 2, 1, '2025-12-11 16:46:23', '2025-12-11 16:46:23'),
(3, 'professor1@test.com', '$2a$11$fMhyZV7365d3nQpCA0Q2sOQzXbmQFLa9DgTOBr9IICwL9aztlDGQa', 'PROFESOR', 'UNO', 'TEST', NULL, 1, 1, '2025-12-11 16:46:59', '2025-12-15 22:09:25'),
(4, 'professor2@test.com', '$2a$11$oLEGuEaTpfNCNhVV3uHMo.ZPfgQjPb0IUPeo57NjLkwX/ppx/FhQu', 'PROFESOR', 'DOS', 'TEST', NULL, 1, 1, '2025-12-11 16:54:18', '2025-12-15 22:09:42'),
(5, 'professor3@test.com', '$2a$11$I99r4y9x1St28H5tATjKG.PjnPV.w29JP.0TtDv8Njvx263JVkJ4i', 'PROFESOR', 'TRES', 'TEST', NULL, 1, 1, '2025-12-11 16:55:45', '2025-12-11 16:56:37'),
(6, 'professor4@test.com', '$2a$11$ColrpzLJKIZ0DgdMKZ8nPOnLkSTYquEtoMgTn3O4zNtI3g9X2xoDC', 'PROFESOR', 'CUATRO', 'TEST', NULL, 1, 1, '2025-12-11 16:56:30', '2025-12-11 16:56:30'),
(7, 'professor5@test.com', '$2a$11$d2ZLJlYwvVantZyeSulzbuWBfDLNXGrSY0gOqxN4lRcJzR.DGXdJG', 'PROFESOR', 'CINCO', 'TEST', NULL, 1, 1, '2025-12-11 16:57:24', '2025-12-11 16:57:24'),
(8, 'student1@test.com', '$2a$11$.FabSiUT5H6TOmbvtS9GIOafUvl.k76RjZqwfFfBrozuDez1JPcdO', 'ESTUDIANTE', 'UNO', 'TEST', NULL, 0, 1, '2025-12-11 18:04:17', '2025-12-15 02:24:44'),
(9, 'student2@test.com', '$2a$11$X3qY5SiElsCpbM.NyCQldOQ1hSS7y2AWLZkF1V1VMW8cmWO6l9ZD6', 'ESTUDIANTE', 'DOS', 'TEST', NULL, 0, 1, '2025-12-11 18:04:44', '2025-12-11 18:04:44'),
(10, 'student3@test.com', '$2a$11$4cDWEJlxalkVhgn/XfZGxuwtKm1ReELm1CJLFQvaooUDHHHjFb2Eq', 'ESTUDIANTE', 'TRES', 'TEST', NULL, 0, 1, '2025-12-11 18:05:06', '2025-12-11 18:05:06'),
(13, 'student4@test.com', '$2a$11$5//WMO8ZzujoL0qft01/k./cA5S7n4doEa8yd3yAMePmE4L1wPkgy', 'ESTUDIANTE', 'CUATRO', 'TEST', NULL, 0, 1, '2025-12-11 18:42:38', '2025-12-11 18:42:38'),
(14, 'student5@test.com', '$2a$11$uPToyq0VnlFHsfCdiCbHI.4vdDlR7bw5dkR5aivF02DMH7QDxhPh2', 'ESTUDIANTE', 'CINCO', 'TEST', NULL, 0, 1, '2025-12-11 18:43:04', '2025-12-11 18:43:04'),
(16, 'student6@test.com', '$2a$11$Rh3EDHxlYGjpYtKx6YzOR.bf3PB5exoHMTNAWIeoJaTc0dFdr8b/O', 'ESTUDIANTE', 'SEIS', 'TEST', NULL, 0, 1, '2025-12-15 18:45:54', '2025-12-15 18:45:54');

-- ==========================================================
-- 2. TABLA: classes
-- ==========================================================
INSERT INTO classes (class_id, class_name, section, subject, color, active, created_at, modified_at) VALUES
('3r2FtCloeA4hcNB', 'Historia Universal', NULL, 'Historia ', '#AD1457', 1, '2025-12-11 16:57:42', '2025-12-11 16:57:42'),
('7iv6GeT7q9TMjWs', 'Fotografía', NULL, 'Fotografía Básica', '#4527A0', 1, '2025-12-11 16:57:10', '2025-12-11 16:57:10'),
('Bv16aDp9yUnTHWt', 'Historia', NULL, 'Historia de México', '#D84315', 1, '2025-12-11 16:51:04', '2025-12-11 16:51:04'),
('dQknIPw3X6TyRW1', 'Historia del Arte', NULL, 'Movimientos y Vanguardias artisticas', '#1565C0', 1, '2025-12-11 16:54:29', '2025-12-11 16:54:29'),
('EH9hbafU0n70irJ', 'Ortografía y Redacción', NULL, 'Ortografía y Redacción', '#1565C0', 1, '2025-12-11 16:55:50', '2025-12-11 16:55:50'),
('GiGfHjXo2QawFNC', 'Biología', NULL, 'Ciencias Naturales', '#2E7D32', 1, '2025-12-11 16:56:23', '2025-12-11 16:56:31'),
('yLAefFseI5dyup0', 'Ciencias sociales', NULL, 'cultura y sociedad', '#6A1B9A', 1, '2025-12-11 16:51:52', '2025-12-11 16:51:52');

-- ==========================================================
-- 3. TABLA: agenda_contacts
-- ==========================================================
INSERT INTO agenda_contacts (agenda_owner_id, contact_id, alias, notes, created_at, modified_at) VALUES
(2, 3, 'Laura', '', '2025-12-11 22:08:38', '2025-12-11 22:08:38'),
(2, 4, 'Enrique Fernández ', '', '2025-12-11 17:01:07', '2025-12-11 17:01:07'),
(2, 5, 'Profe Pepito', '', '2025-12-11 17:03:03', '2025-12-11 17:03:03'),
(2, 6, 'Paola Rojas', '', '2025-12-11 17:03:52', '2025-12-11 17:03:52'),
(2, 7, 'Leo', '', '2025-12-11 17:05:39', '2025-12-11 17:05:39'),
(3, 2, 'A2', '', '2025-12-11 20:56:33', '2025-12-11 20:57:09'),
(3, 4, 'Jose Luis Hernandez', '', '2025-12-15 22:56:56', '2025-12-15 22:56:56'),
(3, 5, 'Profe Pepito', '', '2025-12-11 21:27:05', '2025-12-15 23:57:08'),
(3, 7, 'Leo', '', '2025-12-12 04:34:51', '2025-12-12 04:34:51');

-- ==========================================================
-- 4. TABLA: tags
-- ==========================================================
INSERT INTO tags (tag_id, text, created_at) VALUES
(1, 'FER', '2025-12-12 04:21:49'),
(2, 'LAU', '2025-12-12 04:21:59'),
(3, 'RED', '2025-12-12 04:25:01'),
(5, 'LION', '2025-12-12 04:26:16'),
(7, 'LEO', '2025-12-12 04:27:51'),
(8, 'LU', '2025-12-15 22:50:38'),
(9, 'SEED', '2025-12-15 23:22:43'),
(10, 'ROUGE', '2025-12-15 23:45:38');

-- ==========================================================
-- 5. TABLA: contact_tags
-- ==========================================================
INSERT INTO contact_tags (tag_id, agenda_owner_id, contact_id, created_at) VALUES
(1, 2, 4, '2025-12-12 04:21:49'),
(2, 2, 3, '2025-12-12 04:21:59'),
(5, 3, 7, '2025-12-12 04:34:51'),
(8, 3, 4, '2025-12-15 22:56:56'),
(9, 3, 5, '2025-12-15 23:22:43');

-- ==========================================================
-- 6. TABLA: class_professors
-- ==========================================================
INSERT INTO class_professors (class_id, professor_id, is_owner, created_at) VALUES
('3r2FtCloeA4hcNB', 2, 1, '2025-12-11 16:57:42'),
('3r2FtCloeA4hcNB', 5, 1, '2025-12-11 17:06:17'),
('7iv6GeT7q9TMjWs', 2, 1, '2025-12-11 16:57:10'),
('7iv6GeT7q9TMjWs', 3, 0, '2025-12-11 17:06:30'),
('7iv6GeT7q9TMjWs', 7, 0, '2025-12-11 17:06:30'),
('Bv16aDp9yUnTHWt', 2, 1, '2025-12-11 16:51:04'),
('Bv16aDp9yUnTHWt', 4, 1, '2025-12-11 17:07:58'),
('Bv16aDp9yUnTHWt', 5, 0, '2025-12-11 17:07:58'),
('dQknIPw3X6TyRW1', 2, 1, '2025-12-11 16:54:29'),
('dQknIPw3X6TyRW1', 3, 0, '2025-12-11 17:07:08'),
('EH9hbafU0n70irJ', 2, 1, '2025-12-11 16:55:50'),
('EH9hbafU0n70irJ', 7, 1, '2025-12-11 17:06:59'),
('GiGfHjXo2QawFNC', 2, 1, '2025-12-11 16:56:23'),
('GiGfHjXo2QawFNC', 4, 1, '2025-12-11 17:06:48'),
('yLAefFseI5dyup0', 2, 1, '2025-12-11 16:51:52'),
('yLAefFseI5dyup0', 4, 1, '2025-12-11 17:07:33'),
('yLAefFseI5dyup0', 6, 1, '2025-12-11 17:07:33');

-- ==========================================================
-- 7. TABLA: class_students
-- ==========================================================
INSERT INTO class_students (class_id, student_id, hidden, created_at) VALUES
('3r2FtCloeA4hcNB', 8, 0, '2025-12-11 18:41:51'),
('3r2FtCloeA4hcNB', 9, 0, '2025-12-11 18:46:07'),
('3r2FtCloeA4hcNB', 10, 0, '2025-12-11 18:47:15'),
('3r2FtCloeA4hcNB', 13, 0, '2025-12-11 18:53:39'),
('7iv6GeT7q9TMjWs', 10, 0, '2025-12-11 18:47:28'),
('7iv6GeT7q9TMjWs', 13, 0, '2025-12-11 18:49:53'),
('7iv6GeT7q9TMjWs', 14, 0, '2025-12-11 18:55:07'),
('Bv16aDp9yUnTHWt', 8, 0, '2025-12-11 18:42:15'),
('Bv16aDp9yUnTHWt', 10, 0, '2025-12-11 18:48:38'),
('Bv16aDp9yUnTHWt', 13, 0, '2025-12-11 18:49:32'),
('dQknIPw3X6TyRW1', 8, 0, '2025-12-11 18:42:30'),
('dQknIPw3X6TyRW1', 10, 0, '2025-12-11 18:48:09'),
('dQknIPw3X6TyRW1', 13, 0, '2025-12-11 18:53:09'),
('EH9hbafU0n70irJ', 8, 0, '2025-12-11 18:42:52'),
('EH9hbafU0n70irJ', 10, 0, '2025-12-11 18:47:57'),
('EH9hbafU0n70irJ', 13, 0, '2025-12-11 18:50:51'),
('GiGfHjXo2QawFNC', 9, 0, '2025-12-11 18:45:33'),
('GiGfHjXo2QawFNC', 10, 0, '2025-12-11 18:47:41'),
('GiGfHjXo2QawFNC', 14, 0, '2025-12-11 18:54:49'),
('yLAefFseI5dyup0', 9, 0, '2025-12-11 18:46:28'),
('yLAefFseI5dyup0', 10, 0, '2025-12-11 18:48:24');

-- ==========================================================
-- 8. TABLA: resources
-- ==========================================================
INSERT INTO resources (resource_id, professor_id, title, content, color, active, created_at, modified_at) VALUES
(UNHEX(REPLACE('07cf8f74-4d8e-4326-88c0-3bdc61f1470d', '-', '')), 3, 'El principito', '[{"id":"eda26c64-1cbe-4cab-bb2b-59674ad770af","type":"image","props":{"textAlignment":"left","backgroundColor":"default","name":"el-principito-ilustracion-portada-libro-1024x537.jpg","url":"https://www.ceu.es/blog/wp-content/uploads/2024/08/el-principito-ilustracion-portada-libro-1024x537.jpg","caption":"","showPreview":true},"children":[]},{"id":"412309e7-071d-4723-b56e-be4939928bfa","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#2E7D32', 1, '2025-12-12 07:47:57', '2025-12-12 07:48:47'),
(UNHEX(REPLACE('0eeb0c08-c98a-4546-b072-fc09b328b4cb', '-', '')), 2, 'Mitocondrias', '[{"id":"d3f0fc73-484e-4729-bc49-777714ebff0c","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#2E7D32', 1, '2025-12-11 17:18:34', '2025-12-11 17:18:48'),
(UNHEX(REPLACE('1723862b-723e-4680-8b3b-5115def8d041', '-', '')), 2, 'Las Tortugas Ninja del Renacimiento.', '[{"id":"9551cf01-182b-4049-9b86-a3ff9e8a2eec","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#4527A0', 1, '2025-12-11 17:17:30', '2025-12-11 17:18:28'),
(UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 2, 'Para maestros', '[{"id":"4b0c1fa8-d7df-4e84-b2e7-bf26a05c98e8","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[{"type":"text","text":"Llegar a tiempo a las juntas programadas, gracias.","styles":{}}],"children":[]},{"id":"c7b9136c-99bf-4041-a0b6-2d005f359af6","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#C62828', 1, '2025-12-11 17:33:11', '2025-12-11 17:36:44'),
(UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 2, 'Avisos Académicos', '[{"id":"4a9b644f-07af-434b-b6ee-bb482ae0e825","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[{"type":"text","text":"Para úitimos semestres","styles":{"bold":true}}],"children":[]},{"id":"b9202ff3-82e1-47dc-9124-1098f8141cfd","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#D84315', 1, '2025-12-11 17:34:35', '2025-12-11 17:35:11'),
(UNHEX(REPLACE('5211528a-7b3b-484c-b00b-aa3c4df203e9', '-', '')), 2, 'Historia de la Batalla de Puebla ', '[{"id":"b67b3444-3d10-43d9-bf3b-b703286dbf2f","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[{"type":"text","text":"HistoriaPue","styles":{"bold":true}}],"children":[]},{"id":"ff312227-fc79-445a-98f3-f283d91d10f4","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#2E7D32', 1, '2025-12-11 17:09:28', '2025-12-11 17:13:58'),
(UNHEX(REPLACE('53cc58b2-89d2-414b-9348-01e4888b7210', '-', '')), 3, 'Renacimineto', '[{"id":"8ca249c2-109d-40b5-a23b-c90cebae4127","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#C62828', 1, '2025-12-17 19:12:39', '2025-12-17 19:38:16'),
(UNHEX(REPLACE('5825bf44-9fa1-4ded-a2df-4556e9fd586c', '-', '')), 2, 'Agudas, Graves y esdrújulas.', '[{"id":"e4bf260d-df06-4ae7-aa14-6ec3169dc92b","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#1976d2', 1, '2025-12-11 17:27:34', '2025-12-11 17:28:29'),
(UNHEX(REPLACE('6fb41542-544d-431d-9da3-3b16734e2460', '-', '')), 2, 'Ecuaciones ', '[{"id":"6aff3f3b-b54f-49a6-a4ce-e5ace61b1103","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[{"type":"text","text":"(","styles":{}},{"type":"text","text":"+1 punto quien las resuelva)","styles":{"italic":true,"underline":true}}],"children":[]},{"id":"d9f5967d-5ed2-47cb-af86-8379f6317f61","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#AD1457', 1, '2025-12-11 17:25:38', '2025-12-11 17:27:29'),
(UNHEX(REPLACE('7ea0b5dc-aa7d-4cec-8c07-c754b7388cfc', '-', '')), 2, 'Nueva nota', '[{"id":"99adef03-4838-4d11-ad45-5af20f9c065e","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#1976d2', 1, '2025-12-11 17:30:08', '2025-12-11 17:30:08'),
(UNHEX(REPLACE('c44ee46d-280f-4320-be0c-123f830316ce', '-', '')), 2, 'Ecuaciones Diferenciales', '[{"id":"0c487278-15cf-442f-bbce-cd11d9ddd750","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#C62828', 1, '2025-12-11 17:16:07', '2025-12-11 17:16:39'),
(UNHEX(REPLACE('d6941b79-3c02-495a-a1fe-6e4f35c01082', '-', '')), 3, 'juju', '[{"id":"6fb14efd-bee0-414b-ae72-d8b9043aeae6","type":"image","props":{"textAlignment":"left","backgroundColor":"default","name":"icono-vectorial-dibujos-animados-ilustracion-naturaleza-icono-vacaciones-aislado-plano_138676-13304.jpg?semt=ais_hybrid&w=740&q=80","url":"https://img.freepik.com/vector-gratis/icono-vectorial-dibujos-animados-ilustracion-naturaleza-icono-vacaciones-aislado-plano_138676-13304.jpg?semt=ais_hybrid&w=740&q=80","caption":"","showPreview":true},"children":[]},{"id":"fa5038ee-5ac4-404b-9e9e-ccb1bdf2dd82","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#6A1B9A', 1, '2025-12-11 17:15:24', '2025-12-11 22:00:07'),
(UNHEX(REPLACE('dbcd5fc9-f032-4058-8141-f75f32ff5491', '-', '')), 2, 'Reglas ortográficas', '[{"id":"9e4e253c-84f0-421d-b79c-732dbe2f4c88","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#AD1457', 1, '2025-12-11 17:14:51', '2025-12-11 17:15:24'),
(UNHEX(REPLACE('e12fb986-9da3-4a5b-b9aa-4e92a33bb4c6', '-', '')), 2, 'Proporción Áurea', '[{"id":"fed3e985-af71-475a-adbc-13ca47629da6","type":"paragraph","props":{"backgroundColor":"default","textColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#1976d2', 1, '2025-12-11 17:12:03', '2025-12-11 17:13:42'),
(UNHEX(REPLACE('e51ced50-cfcd-4136-9657-ef88292ee96d', '-', '')), 2, 'Francisco Fernando.', '[{"id":"fe7ee85e-0883-436e-a1a1-8f78401ea3e1","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#1565C0', 1, '2025-12-11 17:29:00', '2025-12-11 17:29:27'),
(UNHEX(REPLACE('ecb79bf8-ec85-46dd-b781-9a96f34d40cd', '-', '')), 2, 'Alianzas en la Segunda Guerra Mundial.', '[{"id":"7cf02411-68a0-4b07-94d4-5e7025f9741b","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#00695C', 1, '2025-12-11 17:16:44', '2025-12-11 17:17:24'),
(UNHEX(REPLACE('eea6a294-e3c9-4906-9b86-33d79b5073ef', '-', '')), 2, 'Derechos Humanos ', '[{"id":"595182f7-ae9a-494e-900e-abfc8444aaea","type":"paragraph","props":{"textColor":"default","backgroundColor":"default","textAlignment":"left"},"content":[],"children":[]}]', '#D84315', 1, '2025-12-11 17:14:02', '2025-12-11 17:14:36');

-- ==========================================================
-- 9. TABLA: class_resources
-- ==========================================================
INSERT INTO class_resources (class_id, resource_id, hidden, created_at) VALUES
('3r2FtCloeA4hcNB', UNHEX(REPLACE('1723862b-723e-4680-8b3b-5115def8d041', '-', '')), 0, '2025-12-11 18:56:21'),
('3r2FtCloeA4hcNB', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('3r2FtCloeA4hcNB', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('3r2FtCloeA4hcNB', UNHEX(REPLACE('e51ced50-cfcd-4136-9657-ef88292ee96d', '-', '')), 0, '2025-12-11 17:29:35'),
('3r2FtCloeA4hcNB', UNHEX(REPLACE('ecb79bf8-ec85-46dd-b781-9a96f34d40cd', '-', '')), 0, '2025-12-11 17:25:05'),
('7iv6GeT7q9TMjWs', UNHEX(REPLACE('1723862b-723e-4680-8b3b-5115def8d041', '-', '')), 0, '2025-12-11 18:56:21'),
('7iv6GeT7q9TMjWs', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('7iv6GeT7q9TMjWs', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('7iv6GeT7q9TMjWs', UNHEX(REPLACE('e12fb986-9da3-4a5b-b9aa-4e92a33bb4c6', '-', '')), 0, '2025-12-11 17:24:33'),
('Bv16aDp9yUnTHWt', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('Bv16aDp9yUnTHWt', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('Bv16aDp9yUnTHWt', UNHEX(REPLACE('5211528a-7b3b-484c-b00b-aa3c4df203e9', '-', '')), 0, '2025-12-11 17:23:00'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('07cf8f74-4d8e-4326-88c0-3bdc61f1470d', '-', '')), 0, '2025-12-12 07:48:41'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('1723862b-723e-4680-8b3b-5115def8d041', '-', '')), 0, '2025-12-11 18:56:59'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('53cc58b2-89d2-414b-9348-01e4888b7210', '-', '')), 0, '2025-12-17 19:23:28'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('7ea0b5dc-aa7d-4cec-8c07-c754b7388cfc', '-', '')), 0, '2025-12-11 17:33:02'),
('dQknIPw3X6TyRW1', UNHEX(REPLACE('e12fb986-9da3-4a5b-b9aa-4e92a33bb4c6', '-', '')), 0, '2025-12-11 17:24:33'),
('EH9hbafU0n70irJ', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('EH9hbafU0n70irJ', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('EH9hbafU0n70irJ', UNHEX(REPLACE('7ea0b5dc-aa7d-4cec-8c07-c754b7388cfc', '-', '')), 0, '2025-12-11 17:31:37'),
('EH9hbafU0n70irJ', UNHEX(REPLACE('dbcd5fc9-f032-4058-8141-f75f32ff5491', '-', '')), 0, '2025-12-11 17:46:06'),
('GiGfHjXo2QawFNC', UNHEX(REPLACE('0eeb0c08-c98a-4546-b072-fc09b328b4cb', '-', '')), 0, '2025-12-11 18:56:08'),
('GiGfHjXo2QawFNC', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('GiGfHjXo2QawFNC', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('GiGfHjXo2QawFNC', UNHEX(REPLACE('c44ee46d-280f-4320-be0c-123f830316ce', '-', '')), 0, '2025-12-11 18:59:48'),
('yLAefFseI5dyup0', UNHEX(REPLACE('0eeb0c08-c98a-4546-b072-fc09b328b4cb', '-', '')), 0, '2025-12-11 18:56:08'),
('yLAefFseI5dyup0', UNHEX(REPLACE('210a66e6-c8cc-4b97-9273-cf7705212d28', '-', '')), 0, '2025-12-11 17:34:02'),
('yLAefFseI5dyup0', UNHEX(REPLACE('366b0c1b-4c48-46d5-86de-67072eb427db', '-', '')), 0, '2025-12-11 17:35:07'),
('yLAefFseI5dyup0', UNHEX(REPLACE('eea6a294-e3c9-4906-9b86-33d79b5073ef', '-', '')), 0, '2025-12-11 17:25:18');

-- ==========================================================
-- 10. TABLA: tests
-- ==========================================================
INSERT INTO tests (test_id, professor_id, title, content, time_limit_minutes, color, active, created_at, modified_at) VALUES
(UNHEX(REPLACE('08de38dc-0b16-4024-8c9a-87c67a24a2d6', '-', '')), 2, 'Exámen ecuaciones diferenciales.', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#C62828', 1, '2025-12-11 17:38:06', '2025-12-11 17:41:46'),
(UNHEX(REPLACE('08de38dc-91f0-4665-863a-dbc7c530348c', '-', '')), 2, 'Vanguardias artisticas y sus exponentes', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#4527A0', 1, '2025-12-11 17:41:52', '2025-12-11 17:48:15'),
(UNHEX(REPLACE('08de38dc-c794-4710-8d48-2b8b1470b437', '-', '')), 2, 'Nueva Evaluación', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#1976d2', 1, '2025-12-11 17:43:22', '2025-12-11 17:43:22'),
(UNHEX(REPLACE('08de38dd-37fc-4326-8e78-bc873f95c577', '-', '')), 2, 'Biología 1', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#2E7D32', 1, '2025-12-11 17:46:31', '2025-12-11 17:48:05'),
(UNHEX(REPLACE('08de38dd-7ba7-4a3f-81c9-e7992fe4bcc0', '-', '')), 2, 'Reglas ortogáficas básicas.', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#C62828', 1, '2025-12-11 17:48:24', '2025-12-11 17:49:44'),
(UNHEX(REPLACE('08de38e3-de21-42ce-85fa-fed2ed020c97', '-', '')), 2, 'Artículos de la constitución', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#00695C', 1, '2025-12-11 18:34:06', '2025-12-11 18:35:59'),
(UNHEX(REPLACE('08de38e4-271c-4e7d-8b5e-c7efbc1ae676', '-', '')), 2, 'Segunda Guerra Mundial', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#AD1457', 1, '2025-12-11 18:36:09', '2025-12-11 18:38:35'),
(UNHEX(REPLACE('08de39bb-9052-4f1b-861c-adc9cd2ad10a', '-', '')), 3, 'Triangulo de exposición y encuadre', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#1976d2', 1, '2025-12-12 20:18:07', '2025-12-12 20:21:13'),
(UNHEX(REPLACE('08de3dbe-0421-4fb4-8af7-f27b6e5f3e4c', '-', '')), 1, 'Nueva Evaluación', '{"55578ae2-c271-40ea-a7ef-e68e8e237177":{"type":"open","Title":"Nueva pregunta","ImageUrl":null}}', NULL, '#1976d2', 1, '2025-12-17 22:45:45', '2025-12-17 22:45:45');

-- ==========================================================
-- 11. TABLA: tests_per_class
-- ==========================================================
INSERT INTO tests_per_class (test_id, class_id, created_at) VALUES
(UNHEX(REPLACE('08de38dc-c794-4710-8d48-2b8b1470b437', '-', '')), 'yLAefFseI5dyup0', '2025-12-11 17:45:29'),
(UNHEX(REPLACE('08de38dd-37fc-4326-8e78-bc873f95c577', '-', '')), 'GiGfHjXo2QawFNC', '2025-12-11 17:47:46'),
(UNHEX(REPLACE('08de38dd-7ba7-4a3f-81c9-e7992fe4bcc0', '-', '')), 'EH9hbafU0n70irJ', '2025-12-11 17:49:33'),
(UNHEX(REPLACE('08de38e3-de21-42ce-85fa-fed2ed020c97', '-', '')), 'yLAefFseI5dyup0', '2025-12-11 18:35:57'),
(UNHEX(REPLACE('08de38e4-271c-4e7d-8b5e-c7efbc1ae676', '-', '')), '3r2FtCloeA4hcNB', '2025-12-11 18:38:32');

-- ==========================================================
-- 12. TABLA: notifications
-- ==========================================================
INSERT INTO notifications (notification_id, class_id, title, active, created_at) VALUES
(1, 'GiGfHjXo2QawFNC', 'ADMIN TEST ha agregado un nuevo recurso en Biología', 1, '2025-12-11 17:19:14'),
(2, 'Bv16aDp9yUnTHWt', 'ADMIN TEST ha agregado un nuevo recurso en Historia', 1, '2025-12-11 17:22:40'),
(3, '7iv6GeT7q9TMjWs', 'ADMIN TEST ha agregado un nuevo recurso en Fotografía', 1, '2025-12-11 17:22:40'),
(4, 'Bv16aDp9yUnTHWt', 'ADMIN TEST ha agregado un nuevo recurso en Historia', 1, '2025-12-11 17:23:00'),
(5, '7iv6GeT7q9TMjWs', 'ADMIN TEST ha agregado un nuevo recurso en Fotografía', 1, '2025-12-11 17:24:33'),
(6, 'GiGfHjXo2QawFNC', 'ADMIN TEST ha agregado un nuevo recurso en Biología', 1, '2025-12-11 17:34:02'),
(7, 'Bv16aDp9yUnTHWt', 'ADMIN TEST ha agregado un nuevo recurso en Historia', 1, '2025-12-11 17:34:02'),
(8, '7iv6GeT7q9TMjWs', 'ADMIN TEST ha agregado un nuevo recurso en Fotografía', 1, '2025-12-11 17:34:02'),
(9, '7iv6GeT7q9TMjWs', 'ADMIN TEST ha agregado un nuevo recurso en Fotografía', 1, '2025-12-11 17:35:07'),
(10, 'GiGfHjXo2QawFNC', 'ADMIN TEST ha agregado un nuevo recurso en Biología', 1, '2025-12-11 17:35:07'),
(11, 'Bv16aDp9yUnTHWt', 'ADMIN TEST ha agregado un nuevo recurso en Historia', 1, '2025-12-11 17:35:07'),
(12, 'Bv16aDp9yUnTHWt', 'ADMIN TEST ha agregado un nuevo recurso en Historia', 1, '2025-12-11 17:45:48'),
(13, 'yLAefFseI5dyup0', 'ADMIN TEST ha agregado un nuevo recurso en Ciencias sociales', 1, '2025-12-11 17:45:48'),
(14, 'yLAefFseI5dyup0', 'ADMIN TEST ha agregado un nuevo recurso en Ciencias sociales', 1, '2025-12-11 17:46:01'),
(15, 'EH9hbafU0n70irJ', 'ADMIN TEST ha agregado un nuevo recurso en Ortografía y Redacción', 1, '2025-12-11 17:46:06'),
(16, 'yLAefFseI5dyup0', 'ADMIN TEST ha agregado un nuevo recurso en Ciencias sociales', 1, '2025-12-11 18:56:09'),
(17, 'GiGfHjXo2QawFNC', 'ADMIN TEST ha agregado un nuevo recurso en Biología', 1, '2025-12-11 18:56:09'),
(18, '7iv6GeT7q9TMjWs', 'ADMIN TEST ha agregado un nuevo recurso en Fotografía', 1, '2025-12-11 18:56:21'),
(19, '3r2FtCloeA4hcNB', 'ADMIN TEST ha agregado un nuevo recurso en Historia Universal', 1, '2025-12-11 18:56:21'),
(20, 'dQknIPw3X6TyRW1', 'ADMIN TEST ha agregado un nuevo recurso en Historia del Arte', 1, '2025-12-11 18:56:59'),
(23, 'GiGfHjXo2QawFNC', 'ADMIN TEST ha agregado un nuevo recurso en Biología', 1, '2025-12-11 18:59:48'),
(25, 'dQknIPw3X6TyRW1', 'PROFESOR TEST ha agregado un nuevo recurso en Historia del Arte', 1, '2025-12-12 07:45:24'),
(27, 'dQknIPw3X6TyRW1', 'PROFESOR TEST ha agregado un nuevo recurso en Historia del Arte', 1, '2025-12-12 07:48:41'),
(29, 'dQknIPw3X6TyRW1', 'PROFESOR TEST ha agregado un nuevo recurso en Historia del Arte', 1, '2025-12-17 19:23:28');

-- ==========================================================
-- 13. TABLA: notification_per_user
-- ==========================================================
INSERT INTO notification_per_user (notification_id, user_id, readed, created_at, modified_at) VALUES
(16, 9, 0, '2025-12-11 18:56:09', '2025-12-11 18:56:09'),
(16, 10, 0, '2025-12-11 18:56:09', '2025-12-11 18:56:09'),
(17, 9, 0, '2025-12-11 18:56:09', '2025-12-11 18:56:09'),
(17, 10, 0, '2025-12-11 18:56:09', '2025-12-11 18:56:09'),
(17, 14, 1, '2025-12-11 18:56:09', '2025-12-11 18:59:16'),
(18, 10, 0, '2025-12-11 18:56:21', '2025-12-11 18:56:21'),
(18, 13, 0, '2025-12-11 18:56:21', '2025-12-11 18:56:21'),
(18, 14, 1, '2025-12-11 18:56:21', '2025-12-11 18:59:16'),
(19, 8, 0, '2025-12-11 18:56:21', '2025-12-11 18:56:21'),
(19, 9, 0, '2025-12-11 18:56:21', '2025-12-11 18:56:21'),
(19, 10, 0, '2025-12-11 18:56:21', '2025-12-11 18:56:21'),
(19, 13, 0, '2025-12-11 18:56:21', '2025-12-11 18:56:21'),
(20, 8, 0, '2025-12-11 18:56:59', '2025-12-11 18:56:59'),
(20, 10, 0, '2025-12-11 18:56:59', '2025-12-11 18:56:59'),
(20, 13, 0, '2025-12-11 18:56:59', '2025-12-11 18:56:59'),
(23, 9, 0, '2025-12-11 18:59:48', '2025-12-11 18:59:48'),
(23, 10, 0, '2025-12-11 18:59:48', '2025-12-11 18:59:48'),
(23, 14, 1, '2025-12-11 18:59:48', '2025-12-11 18:59:59'),
(25, 8, 1, '2025-12-12 07:45:24', '2025-12-12 19:39:03'),
(25, 10, 0, '2025-12-12 07:45:24', '2025-12-12 07:45:24'),
(25, 13, 0, '2025-12-12 07:45:24', '2025-12-12 07:45:24'),
(27, 8, 0, '2025-12-12 07:48:41', '2025-12-12 07:48:41'),
(27, 10, 0, '2025-12-12 07:48:41', '2025-12-12 07:48:41'),
(27, 13, 0, '2025-12-12 07:48:41', '2025-12-12 07:48:41'),
(29, 8, 0, '2025-12-17 19:23:28', '2025-12-17 19:23:28'),
(29, 10, 0, '2025-12-17 19:23:28', '2025-12-17 19:23:28'),
(29, 13, 0, '2025-12-17 19:23:28', '2025-12-17 19:23:28');
