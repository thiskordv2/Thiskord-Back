CREATE DATABASE thiskord;
GO

USE thiskord;
GO

CREATE TABLE Account
(
    user_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL, -- IDENTITY(1,1) = Auto-incr�mentation de 1 � partir de 1
    user_name VARCHAR(255) NOT NULL,
    user_password VARCHAR(255) NOT NULL, 
    user_mail VARCHAR(255) UNIQUE NOT NULL, -- UNIQUE = Evitez deux fois le m�me mail
    user_picture VARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(), -- Format : yyyy-MM-dd HH:mm:ss[.nnnnnnn], DEFAULT = Valeur de base.
    modified_at DATETIME2,
);

CREATE TABLE Project
(
    project_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    project_name VARCHAR(255) NOT NULL,
    project_desc VARCHAR(MAX), -- MAX = A utiliser quand les tailles d'entr�es de donn�es varient consid�rablement
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2
);

CREATE TABLE Channel
(
    channel_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    channel_name VARCHAR(255) NOT NULL,
    channel_desc VARCHAR(255),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,
);

CREATE TABLE Message
(
    message_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    message_content VARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,
    id_author INT,
    id_channel_author INT,

    CONSTRAINT fk_id_author FOREIGN KEY (id_author) REFERENCES Account(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_channel_author FOREIGN KEY (id_channel_author) REFERENCES Channel(channel_id) ON DELETE CASCADE
);

CREATE TABLE External_Token
(
    xt_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    xt_token VARCHAR(MAX),
    xt_app  VARCHAR(255),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,

    
);

CREATE TABLE Sprint
(
    sprint_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    sprint_goal VARCHAR(255),
    sprint_begin_date DATETIME2,
    sprint_end_date DATETIME2,
    sprint_status VARCHAR(50) DEFAULT 'To Do',
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,

    
);

CREATE TABLE Task
(
    task_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    task_title VARCHAR(255) NOT NULL,
    task_desc VARCHAR(MAX),
    is_subtask BIT DEFAULT 0, -- Bit default, si 0 -> Tache principale, Si 1 -> Sous-T�che
    task_status VARCHAR(50) DEFAULT 'To Do',
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,
    id_creator INT,
    id_resp INT,
    id_project_task INT,
    id_parent_task INT,

    CONSTRAINT fk_id_creator FOREIGN KEY (id_creator) REFERENCES Account(user_id) ON DELETE NO ACTION,
    CONSTRAINT fk_id_resp FOREIGN KEY (id_resp) REFERENCES Account(user_id) ON DELETE NO ACTION,
    CONSTRAINT fk_id_project_task FOREIGN KEY (id_project_task) REFERENCES Project(project_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_parent_task FOREIGN KEY (id_parent_task) REFERENCES Task(task_id) ON DELETE NO ACTION
   
);

        --  Table interm�diare


CREATE TABLE INCLUDE
(   
    id_task INT,
    id_sprint INT,

    CONSTRAINT fk_id_task FOREIGN KEY (id_task) REFERENCES Task(task_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_sprint FOREIGN KEY (id_sprint) REFERENCES Sprint(sprint_id) ON DELETE SET NULL
);

CREATE TABLE ACCESS
(
    is_admin BIT DEFAULT 0,
    is_root BIT DEFAULT 0,
    id_account INT,
    id_project_account INT,
    
    CONSTRAINT fk_id_account FOREIGN KEY (id_account) REFERENCES Account(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_project_account FOREIGN KEY (id_project_account) REFERENCES Project(project_id) ON DELETE SET NULL
);

CREATE TABLE CONTAIN
(
    id_project_xt INT,
    id_xt INT,

    CONSTRAINT fk_id_project_xt FOREIGN KEY (id_project_xt) REFERENCES Project(project_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_xt FOREIGN KEY (id_xt) REFERENCES External_Token(xt_id) ON DELETE SET NULL
);

CREATE TABLE BELONGS
(
    id_project_channel INT,
    id_channel_project INT,

    CONSTRAINT fk_id_project_channel FOREIGN KEY (id_project_channel) REFERENCES Project(project_id) ON DELETE CASCADE,
    CONSTRAINT fk_id_channel_project FOREIGN KEY (id_channel_project) REFERENCES Channel(channel_id) ON DELETE SET NULL,
);

CREATE TABLE ALLOW
(
    is_visible BIT DEFAULT 0,
    is_writable BIT DEFAULT 0,
    id_user INT,
    id_channel_user INT,

    CONSTRAINT fk_id_user FOREIGN KEY (id_user) REFERENCES Account(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_channel_user FOREIGN KEY (id_channel_user) REFERENCES Channel(channel_id) ON DELETE SET NULL,
);

   --add table Logs
CREATE TABLE Logs
(
    logs_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    logs_message VARCHAR(255) NOT NULL,
    id_user_logs int,
    
    CONSTRAINT fk_id_user_logs FOREIGN KEY (id_user_logs) REFERENCES Account(user_id) ON DELETE SET NULL,
)

INSERT INTO Account (user_name, user_mail, user_password, user_picture)
VALUES ('EMRE', 'EMRE@EMRE.EMRE', '$2y$10$6R3FgUNLeQqltxOLo5l.g.ljOHC5Idxjhru8Eo5HDj/.aKvG9U7Ba', 'fuzbfuiebzfuizebfuizbeufbzuf');