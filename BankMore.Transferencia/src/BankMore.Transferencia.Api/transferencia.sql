CREATE DATABASE IF NOT EXISTS transferencia
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_general_ci;

USE transferencia;

CREATE TABLE transferencia (
    idtransferencia CHAR(36) PRIMARY KEY, -- identificacao unica da transferencia
    idcontacorrente_origem CHAR(36) NOT NULL, -- conta corrente de origem
    idcontacorrente_destino CHAR(36) NOT NULL, -- conta corrente de destino
    datamovimento CHAR(25) NOT NULL, -- data da transferencia
    valor DECIMAL(15,2) NOT NULL -- valor com duas casas decimais
);

CREATE TABLE idempotencia (
    chave_idempotencia CHAR(36) PRIMARY KEY, -- identificacao chave de idempotencia
    requisicao TEXT,  -- dados de requisicao
    resultado TEXT    -- dados de retorno
);
