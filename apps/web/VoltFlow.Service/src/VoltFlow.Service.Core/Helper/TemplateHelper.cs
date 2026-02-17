using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Helper
{
    public class TemplateHelper
    {

        public static string FormatTemplate(string html, object values)
        {
            // Pobieramy wszystkie właściwości obiektu (np. ClientName, Amount z Twojego Recordu)
            var properties = values.GetType().GetProperties();

            foreach (var prop in properties)
            {
                var placeholder = $"{{{prop.Name}}}"; // Tworzy "{ClientName}"
                var value = prop.GetValue(values)?.ToString() ?? string.Empty;

                // Formatuje wartości typu decimal jako walutę
                if (prop.PropertyType == typeof(decimal))
                {
                    value = ((decimal)prop.GetValue(values)!).ToString("C");
                }

                html = html.Replace(placeholder, value);
            }

            return html;
        }
    }
}
