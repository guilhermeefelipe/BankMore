CREATE DATABASE IF NOT EXISTS contacorrente
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_general_ci;

USE contacorrente;

CREATE TABLE contacorrente (
    idcontacorrente CHAR(37) PRIMARY KEY, -- id da conta corrente (UUID)
    numero INT(10) NOT NULL UNIQUE, -- numero da conta corrente
    nome VARCHAR(100) NOT NULL, -- nome do titular da conta corrente
    CPF VARCHAR(20) NOT NULL, -- nome do titular da conta corrente
    ativo TINYINT(1) NOT NULL DEFAULT 0, -- (0 = inativa, 1 = ativa)
    senha VARCHAR(100) NOT NULL,
    salt VARCHAR(100) NOT NULL,
    CHECK (ativo IN (0,1))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE movimento (
    idmovimento CHAR(37) PRIMARY KEY, -- identificacao unica do movimento
    idcontacorrente CHAR(37) NOT NULL, -- fk para conta corrente
    datamovimento CHAR(25) NOT NULL, -- data do movimento
    tipomovimento CHAR(1) NOT NULL, -- (C = Credito, D = Debito)
    valor DECIMAL(15,2) NOT NULL, -- duas casas decimais
    CHECK (tipomovimento IN ('C','D')),
    FOREIGN KEY (idcontacorrente) REFERENCES contacorrente(idcontacorrente)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE idempotencia (
    chave_idempotencia CHAR(37) PRIMARY KEY, -- chave de idempotencia
    requisicao TEXT, -- dados da requisicao
    resultado TEXT -- dados de retorno
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;