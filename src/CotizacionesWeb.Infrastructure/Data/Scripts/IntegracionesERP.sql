/* =========================================================
   INTEGRACION ERP - COTIZACIONES WEB
   Reemplazar {{ESQUEMA_ERP}} por el esquema real, por ejemplo:
   CRCC01
   ========================================================= */

IF SCHEMA_ID('{{ESQUEMA_ERP}}') IS NULL
BEGIN
    RAISERROR('El esquema {{ESQUEMA_ERP}} no existe en esta base de datos.', 16, 1);
    RETURN;
END
GO

IF OBJECT_ID('[{{ESQUEMA_ERP}}].COTWEB_PEDIDO_STG', 'V') IS NOT NULL
BEGIN
    DROP VIEW [{{ESQUEMA_ERP}}].[vCotWebInformacionProductosERP];
END
GO

CREATE VIEW [{{ESQUEMA_ERP}}].vCotWebInformacionProductosERP(
Producto,
Descripcion,
CodigoImpuesto,
Tarifa,
Tipo,
Porcentaje,
Precio,
NivelPrecio,
Moneda
)
AS
WITH PreciosVigentes AS
(
    SELECT
        artprec.ARTICULO,
        artprec.PRECIO,
        artprec.NIVEL_PRECIO,
        artprec.MONEDA,
        artprec.VERSION,
        ROW_NUMBER() OVER
        (
            PARTITION BY artprec.ARTICULO, artprec.NIVEL_PRECIO, artprec.MONEDA
            ORDER BY artprec.VERSION DESC
        ) AS RN
    FROM [{{ESQUEMA_ERP}}].ARTICULO_PRECIO artprec
    WHERE artprec.FECHA_INICIO <= CONVERT(DATE, GETDATE())
      AND artprec.FECHA_FIN    >= CONVERT(DATE, GETDATE())
)
SELECT 
    art.ARTICULO,
    art.DESCRIPCION,
    art.IMPUESTO,
    imp.TIPO_TARIFA1,
    imp.TIPO_IMPUESTO1,
    imp.IMPUESTO1,
    pv.PRECIO,
    pv.NIVEL_PRECIO,
    CASE pv.MONEDA
        WHEN 'D' THEN 'USD'
        ELSE 'CRC'
    END AS MONEDA
FROM [{{ESQUEMA_ERP}}].ARTICULO art
INNER JOIN [{{ESQUEMA_ERP}}].IMPUESTO imp 
    ON art.IMPUESTO = imp.IMPUESTO
LEFT JOIN PreciosVigentes pv
    ON art.ARTICULO = pv.ARTICULO
   AND pv.RN = 1
GO


/* =========================================================
   STAGING ERP - COTIZACIONES WEB
   ========================================================= */

/* =========================================================
   TABLA: {{ESQUEMA_ERP}}.COTWEB_PEDIDO_STG
   ========================================================= */
IF OBJECT_ID('{{ESQUEMA_ERP}}.COTWEB_PEDIDO_STG', 'U') IS NOT NULL
BEGIN
    DROP TABLE [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_STG];
END
GO

CREATE TABLE [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_STG]
(
    [LOTE_ID]             UNIQUEIDENTIFIER NOT NULL,
    [TIPO_DOCUMENTO]      VARCHAR(10)      NOT NULL,
    [CLIENTE]             VARCHAR(20)      NOT NULL,
    [CONDICION_PAGO]      VARCHAR(10)      NOT NULL,
    [BODEGA]              VARCHAR(10)      NOT NULL,
    [MONEDA]              VARCHAR(1)      NOT NULL,
    [NIVEL_PRECIO]        VARCHAR(12)      NOT NULL,
    [TIPO_CAMBIO]         DECIMAL(18,6)    NOT NULL,
    [USUARIO_ERP]         VARCHAR(20)      NOT NULL,
    [ACTIVIDAD_COMERCIAL] VARCHAR(20)      NOT NULL,
    [OBSERVACIONES]       VARCHAR(500)     NULL,
    [FECHA_CREACION]      DATETIME         NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_COTWEB_PEDIDO_STG_LOTE_ID]
    ON [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_STG] ([LOTE_ID]);
GO


/* =========================================================
   TABLA: {{ESQUEMA_ERP}}.COTWEB_PEDIDO_LINEA_STG
   ========================================================= */
IF OBJECT_ID('{{ESQUEMA_ERP}}.COTWEB_PEDIDO_LINEA_STG', 'U') IS NOT NULL
BEGIN
    DROP TABLE [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_LINEA_STG];
END
GO

CREATE TABLE [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_LINEA_STG]
(
    [LOTE_ID]              UNIQUEIDENTIFIER NOT NULL,
    [LINEA]                INT              NOT NULL,
    [PRODUCTO]             VARCHAR(50)      NOT NULL,
    [DESCRIPCION]          VARCHAR(200)     NULL,
    [CANTIDAD]             DECIMAL(18,4)    NOT NULL,
    [PRECIO_UNITARIO]      DECIMAL(18,2)    NOT NULL,
    [PORCENTAJE_IMPUESTO]  DECIMAL(5,2)     NULL,
    [MONTO_DESCUENTO]      DECIMAL(18,2)    NULL,
    [SUBTOTAL]             DECIMAL(18,2)    NOT NULL,
    [BODEGA]               VARCHAR(10)      NOT NULL
);
GO

CREATE NONCLUSTERED INDEX [IX_COTWEB_PEDIDO_LINEA_STG_LOTE_ID]
    ON [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_LINEA_STG] ([LOTE_ID]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [UX_COTWEB_PEDIDO_LINEA_STG_LOTE_LINEA]
    ON [{{ESQUEMA_ERP}}].[COTWEB_PEDIDO_LINEA_STG] ([LOTE_ID], [LINEA]);
GO


CREATE OR ALTER PROCEDURE {{ESQUEMA_ERP}}.COTWEB_PROCESAR_PEDIDO_STAGE
    @LOTE_ID UNIQUEIDENTIFIER,
    @PEDIDO_GENERADO VARCHAR(50) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        /* =========================================================
           VARIABLES GENERALES
           ========================================================= */
        DECLARE @CONSEC_PEDIDO           VARCHAR(10),
                @TAM_CONSEC              INT,
                @VAL_CONSEC_PED          VARCHAR(50),

                @CLIENTE                 VARCHAR(20),
                @FECHA                   DATETIME,
                @MONEDA                  VARCHAR(1),
                @NIVEL_PRECIO            VARCHAR(12),
                @NOTA                    VARCHAR(500),
                @ACTIVIDAD_COMERCIAL     VARCHAR(20),
                @BODEGA                  VARCHAR(10),
                @USUARIO                 VARCHAR(20),
                @TIPO_CAMBIO             DECIMAL(18,6),
                @CONDICION_PAGO          VARCHAR(10),
                @TIPO_DOCUMENTO          VARCHAR(10),

                @NOMBRE_CLIENTE          VARCHAR(150),
                @PAIS                    VARCHAR(4),
                @ZONA                    VARCHAR(4),
                @RUTA                    VARCHAR(4),
                @VENDEDOR                VARCHAR(4),
                @CODIGO_IMPUESTO_CLI     VARCHAR(4),
                @DIVISION_GEOGRAFICA1    VARCHAR(12),
                @DIVISION_GEOGRAFICA2    VARCHAR(12),
                @DETALLE_DIRECCION       INT,
                @DIRECCION_FACTURA       VARCHAR(4000),
                @DESC_DIREC_EMBARQUE     VARCHAR(250),

                @VERSION_NP              INT,

                @SUB_TOTAL               DECIMAL(28,8),
                @TOTAL_UNID              DECIMAL(28,8),
                @TOTAL_IMPUESTO          DECIMAL(28,8),
                @TOTAL_A_FACTURAR        DECIMAL(28,8),

                @CANT_CABECERA           INT,
                @CANT_LINEAS             INT;

        SET @PEDIDO_GENERADO = NULL;

        /* =========================================================
           VALORES FIJOS / DEFAULTS
           VALIDAR Y AJUSTAR SEGUN SE REQUIERA
           ========================================================= */
        DECLARE @PEDIDO_ESTADO              VARCHAR(1)  = 'N',
                @PEDIDO_IMPRESO             VARCHAR(1)  = 'N',
                @PEDIDO_TIPO_PEDIDO         VARCHAR(1)  = 'N',
                @PEDIDO_AUTORIZADO          VARCHAR(1)  = 'N',
                @PEDIDO_DOC_A_GENERAR       VARCHAR(1)  = 'F',
                @PEDIDO_CLASE_PEDIDO        VARCHAR(1)  = 'N',
                @PEDIDO_COBRADOR            VARCHAR(4)  = 'ND',
                @PEDIDO_BACKORDER           VARCHAR(1)  = 'N',
                @PEDIDO_DESCUENTO_CASCADA   VARCHAR(1)  = 'N',
                @PEDIDO_FIJAR_TIPO_CAMBIO   VARCHAR(1)  = 'N',
                @PEDIDO_ORIGEN_PEDIDO       VARCHAR(1)  = 'F',
                @PEDIDO_PORC_INTCTE         DECIMAL(28,8) = 0,
                @PEDIDO_CONTRATO_REVENTA    VARCHAR(1)  = 'N',
                @PEDIDO_MONTO_OTRO_CARGO    DECIMAL(28,8) = 0,
                @PEDIDO_ES_FACT_REEMPLAZO   VARCHAR(1)  = 'N',
                @PEDIDO_SUBTIPO_DOC_CXC     INT         = 0,
                @PEDIDO_TIPO_DOC_CXC        VARCHAR(3)  = 'FAC',

                @LINEA_ESTADO               VARCHAR(1)  = 'N',
                @LINEA_TIPO_DESCUENTO       VARCHAR(1)  = 'M',
                @LINEA_TIPO_DESC            VARCHAR(10) = '0',
                @LINEA_ES_OTRO_CARGO        VARCHAR(1)  = 'N',
                @LINEA_ES_CANASTA_BASICA    VARCHAR(1)  = 'N';

        /* Ajustar el consecutivo según corresponda*/
        SET @CONSEC_PEDIDO = 'PED';

        /* =========================================================
           VALIDACIONES DE STAGE
           ========================================================= */
        SELECT @CANT_CABECERA = COUNT(*)
        FROM {{ESQUEMA_ERP}}.COTWEB_PEDIDO_STG
        WHERE LOTE_ID = @LOTE_ID;

        IF @CANT_CABECERA = 0
        BEGIN
            RAISERROR('No existe cabecera en COTWEB_PEDIDO_STG para el lote indicado.', 16, 1);
            RETURN;
        END

        IF @CANT_CABECERA > 1
        BEGIN
            RAISERROR('Existe más de una cabecera en COTWEB_PEDIDO_STG para el mismo lote.', 16, 1);
            RETURN;
        END

        SELECT @CANT_LINEAS = COUNT(*)
        FROM {{ESQUEMA_ERP}}.COTWEB_PEDIDO_LINEA_STG
        WHERE LOTE_ID = @LOTE_ID;

        IF @CANT_LINEAS = 0
        BEGIN
            RAISERROR('No existen líneas en COTWEB_PEDIDO_LINEA_STG para el lote indicado.', 16, 1);
            RETURN;
        END

        /* =========================================================
           CARGAR CABECERA DESDE STAGE
           ========================================================= */
        SELECT
            @TIPO_DOCUMENTO      = TIPO_DOCUMENTO,
            @CLIENTE             = CLIENTE,
            @CONDICION_PAGO      = CONDICION_PAGO,
            @BODEGA              = BODEGA,
            @MONEDA              = MONEDA,
            @NIVEL_PRECIO        = NIVEL_PRECIO,
            @TIPO_CAMBIO         = TIPO_CAMBIO,
            @USUARIO             = USUARIO_ERP,
            @ACTIVIDAD_COMERCIAL = ACTIVIDAD_COMERCIAL,
            @NOTA                = OBSERVACIONES,
            @FECHA               = FECHA_CREACION
        FROM {{ESQUEMA_ERP}}.COTWEB_PEDIDO_STG
        WHERE LOTE_ID = @LOTE_ID;

        /* =========================================================
           DATOS COMPLEMENTARIOS DEL CLIENTE ERP
           ========================================================= */
        SELECT
            @NOMBRE_CLIENTE       = C.NOMBRE,
            @DETALLE_DIRECCION    = C.DETALLE_DIRECCION,
            @PAIS                 = C.PAIS,
            @ZONA                 = C.ZONA,
            @RUTA                 = C.RUTA,
            @VENDEDOR             = C.VENDEDOR,
            @CODIGO_IMPUESTO_CLI  = C.CODIGO_IMPUESTO,
            @DIVISION_GEOGRAFICA1 = C.DIVISION_GEOGRAFICA1,
            @DIVISION_GEOGRAFICA2 = C.DIVISION_GEOGRAFICA2
        FROM {{ESQUEMA_ERP}}.CLIENTE C
        WHERE C.CLIENTE = @CLIENTE;

        IF @NOMBRE_CLIENTE IS NULL
        BEGIN
            RAISERROR('No se encontró información del cliente ERP para el cliente indicado en stage.', 16, 1);
            RETURN;
        END

        SELECT
            @DIRECCION_FACTURA = SUBSTRING(
                ISNULL(CAMPO_1,'') + ISNULL(CAMPO_2,'') + ISNULL(CAMPO_3,'') + ISNULL(CAMPO_4,'') +
                ISNULL(CAMPO_5,'') + ISNULL(CAMPO_6,'') + ISNULL(CAMPO_7,'') + ISNULL(CAMPO_8,'') +
                ISNULL(CAMPO_9,'') + ISNULL(CAMPO_10,''),
                1, 4000)
        FROM {{ESQUEMA_ERP}}.DETALLE_DIRECCION
        WHERE DETALLE_DIRECCION = @DETALLE_DIRECCION;

        SET @DESC_DIREC_EMBARQUE =
            CASE
                WHEN LEN(ISNULL(@DIRECCION_FACTURA,'')) > 250
                    THEN SUBSTRING(@DIRECCION_FACTURA, 1, 250)
                ELSE ISNULL(@DIRECCION_FACTURA,'')
            END;

        /* =========================================================
           VERSION NIVEL PRECIO
           ========================================================= */
        SELECT @VERSION_NP = MAX(VN.VERSION)
        FROM {{ESQUEMA_ERP}}.VERSION_NIVEL VN
        WHERE VN.NIVEL_PRECIO = @NIVEL_PRECIO
          AND VN.MONEDA = @MONEDA
          AND VN.ESTADO = 'A';

        IF @VERSION_NP IS NULL
        BEGIN
            RAISERROR('No se encontró una versión activa para el nivel de precio y moneda indicados.', 16, 1);
            RETURN;
        END

        /* =========================================================
           VALIDAR PRODUCTOS EN VISTA ERP
           ========================================================= */
        IF EXISTS
        (
            SELECT 1
            FROM {{ESQUEMA_ERP}}.COTWEB_PEDIDO_LINEA_STG L
            LEFT JOIN {{ESQUEMA_ERP}}.vCotWebInformacionProductosERP V
                ON V.Producto = L.PRODUCTO
            WHERE L.LOTE_ID = @LOTE_ID
              AND V.Producto IS NULL
        )
        BEGIN
            RAISERROR('Uno o más productos del lote no existen en la vista vCotWebInformacionProductosERP.', 16, 1);
            RETURN;
        END

        /* =========================================================
           CONSECUTIVO PEDIDO
           ========================================================= */

        SELECT
            @TAM_CONSEC     = LONGITUD,
            @VAL_CONSEC_PED = VALOR_CONSECUTIVO
        FROM {{ESQUEMA_ERP}}.CONSECUTIVO_FA
        WHERE CODIGO_CONSECUTIVO = @CONSEC_PEDIDO;

        IF @VAL_CONSEC_PED IS NULL
        BEGIN
            RAISERROR('No se logró obtener la información del consecutivo de pedido.', 16, 1);
            RETURN;
        END

        SET @VAL_CONSEC_PED = {{ESQUEMA_ERP}}.NextStrCodigo(@VAL_CONSEC_PED, @TAM_CONSEC);
        SET @PEDIDO_GENERADO = @VAL_CONSEC_PED;

        /* =========================================================
           TRANSACCION
           ========================================================= */
        BEGIN TRANSACTION;

        UPDATE {{ESQUEMA_ERP}}.CONSECUTIVO_FA
        SET VALOR_CONSECUTIVO = @VAL_CONSEC_PED
        WHERE CODIGO_CONSECUTIVO = @CONSEC_PEDIDO;

        /* =========================================================
           INSERT PEDIDO
           VALIDAR Y AJUSTAR CAMPOS DEFAULT SEGUN SE REQUIERA
           ========================================================= */
        INSERT INTO {{ESQUEMA_ERP}}.PEDIDO
        (
            PEDIDO,
            ESTADO,
            FECHA_PEDIDO,
            FECHA_PROMETIDA,
            FECHA_PROX_EMBARQU,
            EMBARCAR_A,
            DIREC_EMBARQUE,
            DIRECCION_FACTURA,
            OBSERVACIONES,
            COMENTARIO_CXC,
            TOTAL_MERCADERIA,
            MONTO_ANTICIPO,
            MONTO_FLETE,
            MONTO_SEGURO,
            MONTO_DOCUMENTACIO,
            TIPO_DESCUENTO1,
            TIPO_DESCUENTO2,
            MONTO_DESCUENTO1,
            MONTO_DESCUENTO2,
            PORC_DESCUENTO1,
            PORC_DESCUENTO2,
            TOTAL_IMPUESTO1,
            TOTAL_IMPUESTO2,
            TOTAL_A_FACTURAR,
            PORC_COMI_VENDEDOR,
            PORC_COMI_COBRADOR,
            TOTAL_CANCELADO,
            TOTAL_UNIDADES,
            IMPRESO,
            FECHA_HORA,
            DESCUENTO_VOLUMEN,
            TIPO_PEDIDO,
            MONEDA_PEDIDO,
            VERSION_NP,
            AUTORIZADO,
            DOC_A_GENERAR,
            CLASE_PEDIDO,
            MONEDA,
            NIVEL_PRECIO,
            COBRADOR,
            RUTA,
            USUARIO,
            CONDICION_PAGO,
            BODEGA,
            ZONA,
            VENDEDOR,
            CLIENTE,
            CLIENTE_DIRECCION,
            CLIENTE_CORPORAC,
            CLIENTE_ORIGEN,
            PAIS,
            BACKORDER,
            DESCUENTO_CASCADA,
            TIPO_CAMBIO,
            FIJAR_TIPO_CAMBIO,
            ORIGEN_PEDIDO,
            DIVISION_GEOGRAFICA1,
            DIVISION_GEOGRAFICA2,
            BASE_IMPUESTO1,
            NOMBRE_CLIENTE,
            TIPO_DOCUMENTO,
            ACTIVIDAD_COMERCIAL,
            PORC_INTCTE,
            DESC_DIREC_EMBARQUE,
            CONTRATO_REVENTA,
            MONTO_OTRO_CARGO,
            ES_FACTURA_REEMPLAZO,
            SUBTIPO_DOC_CXC,
            TIPO_DOC_CXC
        )
        VALUES
        (
            @VAL_CONSEC_PED,
            @PEDIDO_ESTADO,
            @FECHA,
            @FECHA,
            @FECHA,
            @NOMBRE_CLIENTE,
            'ND',
            @DIRECCION_FACTURA,
            @NOTA,
            NULL,
            0,
            0,
            0,
            0,
            0,
            'P',
            'P',
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            0,
            @PEDIDO_IMPRESO,
            GETDATE(),
            0,
            @PEDIDO_TIPO_PEDIDO,
            @MONEDA,
            @VERSION_NP,
            @PEDIDO_AUTORIZADO,
            @PEDIDO_DOC_A_GENERAR,
            @PEDIDO_CLASE_PEDIDO,
            @MONEDA,
            @NIVEL_PRECIO,
            @PEDIDO_COBRADOR,
            @RUTA,
            @USUARIO,
            @CONDICION_PAGO,
            @BODEGA,
            @ZONA,
            @VENDEDOR,
            @CLIENTE,
            @CLIENTE,
            @CLIENTE,
            @CLIENTE,
            @PAIS,
            @PEDIDO_BACKORDER,
            @PEDIDO_DESCUENTO_CASCADA,
            @TIPO_CAMBIO,
            @PEDIDO_FIJAR_TIPO_CAMBIO,
            @PEDIDO_ORIGEN_PEDIDO,
            @DIVISION_GEOGRAFICA1,
            @DIVISION_GEOGRAFICA2,
            0,
            @NOMBRE_CLIENTE,
            @TIPO_DOCUMENTO,
            @ACTIVIDAD_COMERCIAL,
            @PEDIDO_PORC_INTCTE,
            @DESC_DIREC_EMBARQUE,
            @PEDIDO_CONTRATO_REVENTA,
            @PEDIDO_MONTO_OTRO_CARGO,
            @PEDIDO_ES_FACT_REEMPLAZO,
            @PEDIDO_SUBTIPO_DOC_CXC,
            @PEDIDO_TIPO_DOC_CXC
        );

        /* =========================================================
           INSERT PEDIDO_LINEA
           RESPETAR MONTOS DESDE STAGE
           ========================================================= */
        INSERT INTO {{ESQUEMA_ERP}}.PEDIDO_LINEA
        (
            PEDIDO,
            PEDIDO_LINEA,
            BODEGA,
            ARTICULO,
            ESTADO,
            FECHA_ENTREGA,
            LINEA_USUARIO,
            PRECIO_UNITARIO,
            CANTIDAD_PEDIDA,
            CANTIDAD_A_FACTURA,
            CANTIDAD_FACTURADA,
            CANTIDAD_RESERVADA,
            CANTIDAD_BONIFICAD,
            CANTIDAD_CANCELADA,
            TIPO_DESCUENTO,
            MONTO_DESCUENTO,
            PORC_DESCUENTO,
            DESCRIPCION,
            FECHA_PROMETIDA,
            CENTRO_COSTO,
            CUENTA_CONTABLE,
            TIPO_DESC,
            TIPO_IMPUESTO1,
            TIPO_TARIFA1,
            PORC_IMPUESTO1,
            ES_OTRO_CARGO,
            ES_CANASTA_BASICA
        )
        SELECT
            @VAL_CONSEC_PED,      -- PEDIDO - varchar(50)
            CONVERT(SMALLINT, L.LINEA - 1),
            L.BODEGA,
            L.PRODUCTO,
            @LINEA_ESTADO,
            @FECHA,
            CONVERT(SMALLINT, L.LINEA),
            L.PRECIO_UNITARIO,
            L.CANTIDAD,
            L.CANTIDAD,
            0,
            0,
            0,
            0,
            @LINEA_TIPO_DESCUENTO,
            L.MONTO_DESCUENTO,
            0,
            L.DESCRIPCION,
            @FECHA,
            V.CentroCosto,
            V.CuentaContable,
            @LINEA_TIPO_DESC,
            V.Tipo,
            V.Tarifa,
            L.PORCENTAJE_IMPUESTO,
            @LINEA_ES_OTRO_CARGO,
            @LINEA_ES_CANASTA_BASICA
        FROM {{ESQUEMA_ERP}}.COTWEB_PEDIDO_LINEA_STG L
        INNER JOIN {{ESQUEMA_ERP}}.vCotWebInformacionProductosERP V
            ON V.Producto = L.PRODUCTO
        WHERE L.LOTE_ID = @LOTE_ID
        ORDER BY L.LINEA;

        /* =========================================================
           TOTALES
           ========================================================= */
        SELECT
            @SUB_TOTAL = ISNULL(SUM(L.SUBTOTAL), 0),
            @TOTAL_UNID = ISNULL(SUM(L.CANTIDAD), 0),
            @TOTAL_IMPUESTO = ISNULL(SUM(L.SUBTOTAL * (L.PORCENTAJE_IMPUESTO / 100.0)), 0)
        FROM {{ESQUEMA_ERP}}.COTWEB_PEDIDO_LINEA_STG L
        WHERE L.LOTE_ID = @LOTE_ID;

        SET @TOTAL_A_FACTURAR = @SUB_TOTAL + @TOTAL_IMPUESTO;

        UPDATE {{ESQUEMA_ERP}}.PEDIDO
        SET TOTAL_MERCADERIA = @SUB_TOTAL,
            TOTAL_UNIDADES = @TOTAL_UNID,
            TOTAL_IMPUESTO1 = @TOTAL_IMPUESTO,
            TOTAL_A_FACTURAR = @TOTAL_A_FACTURAR,
            BASE_IMPUESTO1 = @SUB_TOTAL
        WHERE PEDIDO = @VAL_CONSEC_PED;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        SET @PEDIDO_GENERADO = NULL;

        DECLARE @ERROR_MESSAGE NVARCHAR(4000),
                @ERROR_SEVERITY INT,
                @ERROR_STATE INT;

        SELECT
            @ERROR_MESSAGE = ERROR_MESSAGE(),
            @ERROR_SEVERITY = ERROR_SEVERITY(),
            @ERROR_STATE = ERROR_STATE();

        RAISERROR(@ERROR_MESSAGE, @ERROR_SEVERITY, @ERROR_STATE);
    END CATCH
END
GO