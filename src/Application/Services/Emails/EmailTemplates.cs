/// <summary>
/// Proporciona plantillas de correo electrónico con formato HTML.
/// </summary>
public static class EmailTemplates
{
    /// <summary>
    /// Genera una plantilla de correo electrónico genérica con un título, mensaje y un botón de acción.
    /// </summary>
    /// <param name="title">El título del correo electrónico.</param>
    /// <param name="mainMessage">El mensaje principal del cuerpo del correo.</param>
    /// <param name="detailLabel">La etiqueta para el campo de detalle (por ejemplo, "Código de Verificación").</param>
    /// <param name="detailValue">El valor del campo de detalle.</param>
    /// <param name="actionText">El texto que se mostrará en el botón de acción.</param>
    /// <param name="actionUrl">La URL a la que redirigirá el botón de acción.</param>
    /// <returns>Una cadena con el contenido HTML del correo electrónico.</returns>
    public static string GetGenericTemplate(
        string title,
        string mainMessage,
        string detailLabel,
        string detailValue,
        string actionText,
        string actionUrl
    )
    {
        return $@"
        <!DOCTYPE html>
        <html lang='es'>
        <head>
            <meta charset='UTF-8'>
            <style>
                body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #333; line-height: 1.6; background-color: #f9f9f9; margin: 0; padding: 0; }}
                .container {{ max-width: 600px; margin: 20px auto; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden; }}
                .header {{ background-color: #0056b3; color: #ffffff; padding: 20px; text-align: center; }}
                .header h1 {{ margin: 0; font-size: 24px; letter-spacing: 1px; }}
                .content {{ padding: 30px; }}
                .content h2 {{ color: #0056b3; margin-top: 0; }}
                .info-box {{ background-color: #f0f7ff; border-left: 4px solid #0056b3; padding: 15px; margin: 20px 0; border-radius: 4px; }}
                .info-box strong {{ color: #0056b3; }}
                .button-container {{ text-align: center; margin-top: 30px; }}
                .button {{ background-color: #28a745; color: #ffffff; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block; }}
                .footer {{ background-color: #f1f1f1; color: #777; padding: 15px; text-align: center; font-size: 12px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1>Edu-zas</h1>
                </div>
                <div class='content'>
                    <h2>{title}</h2>
                    <p>Hola,</p>
                    <p>{mainMessage}</p>
                    
                    <div class='info-box'>
                        <strong>{detailLabel}:</strong> {detailValue}
                    </div>

                    <p>Para más detalles, haz clic en el siguiente botón:</p>
                    
                    <div class='button-container'>
                        <a href='{actionUrl}' class='button'>{actionText}</a>
                    </div>
                </div>
                <div class='footer'>
                    <p>&copy; {DateTime.Now.Year} Edu-zas - Plataforma Educativa</p>
                    <p>Este es un mensaje automático, por favor no respondas.</p>
                </div>
            </div>
        </body>
        </html>";
    }
}
