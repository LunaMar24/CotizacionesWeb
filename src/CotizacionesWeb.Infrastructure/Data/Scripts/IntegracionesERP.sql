CREATE VIEW Softland.vCotWebInformacionProductosERP(
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
    FROM softland.ARTICULO_PRECIO artprec
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
        WHEN 'D' THEN 'DOL'
        ELSE 'CRC'
    END AS MONEDA
FROM softland.ARTICULO art
INNER JOIN softland.IMPUESTO imp 
    ON art.IMPUESTO = imp.IMPUESTO
LEFT JOIN PreciosVigentes pv
    ON art.ARTICULO = pv.ARTICULO
   AND pv.RN = 1;