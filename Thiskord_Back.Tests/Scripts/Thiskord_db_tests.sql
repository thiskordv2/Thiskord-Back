-- Table Account
BEGIN
CREATE TABLE dbo.Account
(
    user_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL, -- IDENTITY(1,1) = Auto-incrémentation
    user_name VARCHAR(255) NOT NULL,
    user_password VARCHAR(255) NOT NULL,
    user_mail VARCHAR(255) UNIQUE NOT NULL,
    user_picture VARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2
);
END

-- Table Project
BEGIN
CREATE TABLE dbo.Project
(
    project_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    project_name VARCHAR(255) NOT NULL,
    project_desc VARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2
);
END

-- Table Channel
BEGIN
CREATE TABLE dbo.Channel
(
    channel_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    channel_name VARCHAR(255) NOT NULL,
    channel_desc VARCHAR(255),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,

    id_project INT,
    
    CONSTRAINT fk_id_project FOREIGN KEY (id_project) REFERENCES dbo.Project(project_id) ON DELETE CASCADE
);
END

-- Table Message (références Account et Channel)
BEGIN
CREATE TABLE dbo.Message
(
    message_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    message_content VARCHAR(MAX),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,
    id_author INT NULL,
    id_channel_author INT NULL,

    CONSTRAINT fk_id_author FOREIGN KEY (id_author) REFERENCES dbo.Account(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_channel_author FOREIGN KEY (id_channel_author) REFERENCES dbo.Channel(channel_id) ON DELETE CASCADE
);
END

-- Table External_Token
BEGIN
CREATE TABLE dbo.External_Token
(
    xt_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    xt_token VARCHAR(MAX),
    xt_app  VARCHAR(255),
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2
);
END

-- Table Sprint
BEGIN
CREATE TABLE dbo.Sprint
(
    sprint_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    sprint_goal VARCHAR(255),
    sprint_begin_date DATETIME2,
    sprint_end_date DATETIME2,
    sprint_status VARCHAR(50) DEFAULT 'To Do',
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2
);
END

-- Table Task (références Account, Project et auto-référence)
BEGIN
CREATE TABLE dbo.Task
(
    task_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    task_title VARCHAR(255) NOT NULL,
    task_desc VARCHAR(MAX),
    is_subtask BIT DEFAULT 0,
    task_status VARCHAR(50) DEFAULT 'To Do',
    created_at DATETIME2 DEFAULT GETDATE(),
    modified_at DATETIME2,
    id_creator INT NULL,
    id_resp INT NULL,
    id_project_task INT NULL,
    id_parent_task INT NULL,

    CONSTRAINT fk_id_creator FOREIGN KEY (id_creator) REFERENCES dbo.Account(user_id) ON DELETE NO ACTION,
    CONSTRAINT fk_id_resp FOREIGN KEY (id_resp) REFERENCES dbo.Account(user_id) ON DELETE NO ACTION,
    CONSTRAINT fk_id_project_task FOREIGN KEY (id_project_task) REFERENCES dbo.Project(project_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_parent_task FOREIGN KEY (id_parent_task) REFERENCES dbo.Task(task_id) ON DELETE NO ACTION
);
END

-- Table INCLUDE (table intermédiaire Task <-> Sprint)
BEGIN
CREATE TABLE dbo.INCLUDE
(
    id_task INT NULL,
    id_sprint INT NULL,

    CONSTRAINT fk_id_task FOREIGN KEY (id_task) REFERENCES dbo.Task(task_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_sprint FOREIGN KEY (id_sprint) REFERENCES dbo.Sprint(sprint_id) ON DELETE SET NULL
);
END

-- Table ACCESS
BEGIN
CREATE TABLE dbo.ACCESS
(
    is_admin BIT DEFAULT 0,
    is_root BIT DEFAULT 0,
    id_account INT NULL,
    id_project_account INT NULL,

    CONSTRAINT fk_id_account FOREIGN KEY (id_account) REFERENCES dbo.Account(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_project_account FOREIGN KEY (id_project_account) REFERENCES dbo.Project(project_id) ON DELETE SET NULL
);
END

-- Table CONTAIN
BEGIN
CREATE TABLE dbo.CONTAIN
(
    id_project_xt INT NULL,
    id_xt INT NULL,

    CONSTRAINT fk_id_project_xt FOREIGN KEY (id_project_xt) REFERENCES dbo.Project(project_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_xt FOREIGN KEY (id_xt) REFERENCES dbo.External_Token(xt_id) ON DELETE SET NULL
);
END

-- Table ALLOW
BEGIN
CREATE TABLE dbo.ALLOW
(
    is_visible BIT DEFAULT 0,
    is_writable BIT DEFAULT 0,
    id_user INT NULL,
    id_channel_user INT NULL,

    CONSTRAINT fk_id_user FOREIGN KEY (id_user) REFERENCES dbo.Account(user_id) ON DELETE SET NULL,
    CONSTRAINT fk_id_channel_user FOREIGN KEY (id_channel_user) REFERENCES dbo.Channel(channel_id) ON DELETE SET NULL
);
END

-- Table Logs
BEGIN
CREATE TABLE dbo.Logs
(
    logs_id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    logs_message VARCHAR(255) NOT NULL,
    id_user_logs INT NULL,

    CONSTRAINT fk_id_user_logs FOREIGN KEY (id_user_logs) REFERENCES dbo.Account(user_id) ON DELETE SET NULL
);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE user_mail = 'EMRE@EMRE.EMRE')
BEGIN
INSERT INTO dbo.Account (user_name, user_mail, user_password, user_picture)
VALUES ('EMRE', 'EMRE@EMRE.EMRE', '$2y$10$6R3FgUNLeQqltxOLo5l.g.ljOHC5Idxjhru8Eo5HDj/.aKvG9U7Ba', 'fuzbfuiebzfuizebfuizbeufbzuf');
END

IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE user_mail = 'rob@rob.rob')
BEGIN
INSERT INTO Account (user_name, user_password, user_mail, user_picture, created_at, modified_at) VALUES ('ROBIN', '$2y$10$S07.pZscIBTQ/o5xZOa0G.zoyHjxDLlfMExURmxQwCyRaw3v38j5C', 'rob@rob.rob', '010101010101010100101', '2025-12-03 11:11:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE user_mail = 'emre@emre.emre')
BEGIN
INSERT INTO Account (user_name, user_password, user_mail, user_picture, created_at, modified_at) VALUES ('EMRE', '$2y$10$sXLDbdQC4jPOKZmCEJdzMeX3VNvHYG1kwg/LKpoVkKEZHq76wQ5NC', 'emre@emre.emre', '010101010101010100101', '2025-12-03 11:20:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Account WHERE user_mail = 'tim@tim.tim')
BEGIN
INSERT INTO Account (user_name, user_password, user_mail, user_picture, created_at, modified_at) VALUES ('TIM', '$2y$10$MGVD./ShFL1supa523qXBuU6.yDvuhxOAoCVo02nK07/BGUjMRrFW', 'tim@tim.tim', '01101010100110', '2025-12-03 11:35:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Project WHERE project_name = 'projet 1')
BEGIN
INSERT INTO Project (project_name, project_desc, created_at, modified_at) VALUES ('projet 1', 'projet super mega cool', '2025-12-03 11:35:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Project WHERE project_name = 'SOURCETREE')
BEGIN
INSERT INTO Project (project_name, project_desc, created_at, modified_at) VALUES ('SOURCETREE', 'Jpréfére google drive', '2025-12-03 11:35:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Project WHERE project_name = 'Projet test')
BEGIN
INSERT INTO Project (project_name, project_desc, created_at, modified_at) VALUES ('Projet test', 'Toutes facons personne ne lis jamais la notice', '2025-12-03 11:35:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 1 AND id_project_account = 1)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 1, 1, 1);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 2 AND id_project_account = 1)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (1, 0, 2, 1);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 3 AND id_project_account = 1)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 0, 3, 1);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 1 AND id_project_account = 2)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 0, 1, 2);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 2 AND id_project_account = 2)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 1, 2, 2);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 3 AND id_project_account = 2)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (1, 0, 3, 2);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 1 AND id_project_account = 3)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (1, 0, 1, 3);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 2 AND id_project_account = 3)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 0, 2, 3);
END
IF NOT EXISTS (SELECT 1 FROM dbo.ACCESS WHERE id_account = 3 AND id_project_account = 3)
BEGIN
INSERT INTO ACCESS (is_admin, is_root, id_account, id_project_account) VALUES (0, 1, 3, 3);
END

IF NOT EXISTS (SELECT 1 FROM dbo.Channel WHERE channel_name = 'Channel 1' AND id_project = 1)
BEGIN
INSERT INTO Channel (channel_name, channel_desc, id_project, created_at, modified_at) VALUES ('Channel 1', 'Personne lis ca bordel', 1, '2025-12-03 11:35:02.7233333', NULL);
END
IF NOT EXISTS (SELECT 1 FROM dbo.Channel WHERE channel_name = 'Channel AAAH' AND id_project = 1)
BEGIN
INSERT INTO Channel (channel_name, channel_desc, id_project, created_at, modified_at) VALUES ('Channel AAAH', 'Qui lis ?', 1, '2025-12-03 11:35:02.7233333', NULL);
END
IF NOT EXISTS (SELECT 1 FROM dbo.Channel WHERE channel_name = 'Channel 1' AND id_project = 2)
BEGIN
INSERT INTO Channel (channel_name, channel_desc, id_project, created_at, modified_at) VALUES ('Channel 1', 'Personne lis ca bordel', 2, '2025-12-03 11:35:02.7233333', NULL);
END
IF NOT EXISTS (SELECT 1 FROM dbo.Channel WHERE channel_name = 'Channel AAAH' AND id_project = 2)
BEGIN
INSERT INTO Channel (channel_name, channel_desc, id_project, created_at, modified_at) VALUES ('Channel AAAH', 'Qui lis ?', 2, '2025-12-03 11:35:02.7233333', NULL);
END
IF NOT EXISTS (SELECT 1 FROM dbo.Channel WHERE channel_name = 'Channel 1' AND id_project = 3)
BEGIN
INSERT INTO Channel (channel_name, channel_desc, id_project, created_at, modified_at) VALUES ('Channel 1', 'Personne lis ca bordel', 3, '2025-12-03 11:35:02.7233333', NULL);
END
IF NOT EXISTS (SELECT 1 FROM dbo.Channel WHERE channel_name = 'Channel AAAH' AND id_project = 3)
BEGIN
INSERT INTO Channel (channel_name, channel_desc, id_project, created_at, modified_at) VALUES ('Channel AAAH', 'Qui lis ?', 3, '2025-12-03 11:35:02.7233333', NULL);
END

IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 1 AND id_channel_user = 1)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (1, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 2 AND id_channel_user = 1)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (2, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 3 AND id_channel_user = 1)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (3, 1, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 1 AND id_channel_user = 2)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (1, 2, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 2 AND id_channel_user = 2)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (2, 2, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 3 AND id_channel_user = 2)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (3, 2, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 1 AND id_channel_user = 3)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (1, 3, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 2 AND id_channel_user = 3)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (2, 3, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 3 AND id_channel_user = 3)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (3, 3, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 1 AND id_channel_user = 4)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (1, 4, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 2 AND id_channel_user = 4)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (2, 4, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 3 AND id_channel_user = 4)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (3, 4, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 1 AND id_channel_user = 5)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (1, 5, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 2 AND id_channel_user = 5)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (2, 5, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 3 AND id_channel_user = 5)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (3, 5, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 1 AND id_channel_user = 6)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (1, 6, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 2 AND id_channel_user = 6)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (2, 6, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.ALLOW WHERE id_user = 3 AND id_channel_user = 6)
    INSERT INTO ALLOW (id_user, id_channel_user, is_visible, is_writable) VALUES (3, 6, 1, 1);

IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Un message' AND id_channel_author = 1)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Un message', '2025-12-03 11:35:02.7233333', NULL, 1, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Une réponse' AND id_channel_author = 1)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Une réponse', '2025-12-03 11:40:02.7233333', NULL, 2, 1);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Un message' AND id_channel_author = 2)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Un message', '2025-12-03 11:35:02.7233333', NULL, 1, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Une réponse' AND id_channel_author = 2)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Une réponse', '2025-12-03 11:40:02.7233333', NULL, 2, 2);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Un message' AND id_channel_author = 3)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Un message', '2025-12-03 11:35:02.7233333', NULL, 1, 3);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Une réponse' AND id_channel_author = 3)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Une réponse', '2025-12-03 11:40:02.7233333', NULL, 2, 3);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Un message' AND id_channel_author = 4)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Un message', '2025-12-03 11:35:02.7233333', NULL, 1, 4);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Une réponse' AND id_channel_author = 4)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Une réponse', '2025-12-03 11:40:02.7233333', NULL, 2, 4);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Un message' AND id_channel_author = 5)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Un message', '2025-12-03 11:35:02.7233333', NULL, 1, 5);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Une réponse' AND id_channel_author = 5)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Une réponse', '2025-12-03 11:40:02.7233333', NULL, 2, 5);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Un message' AND id_channel_author = 6)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Un message', '2025-12-03 11:35:02.7233333', NULL, 1, 6);
IF NOT EXISTS (SELECT 1 FROM dbo.Message WHERE message_content = 'Une réponse' AND id_channel_author = 6)
    INSERT INTO Message (message_content, created_at, modified_at, id_author, id_channel_author) VALUES ('Une réponse', '2025-12-03 11:40:02.7233333', NULL, 2, 6);