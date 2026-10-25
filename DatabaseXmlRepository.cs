using System.Data.Common;
using System.Xml.Linq;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace AdmissionWeb.Data;

/// <summary>Stores ASP.NET Data Protection keys in the configured application database.</summary>
public sealed class DatabaseXmlRepository(AdmissionStore store) : IXmlRepository
{
    public IReadOnlyCollection<XElement> GetAllElements()
    {
        using var connection = store.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Xml FROM DataProtectionKeys ORDER BY Id;";
        using var reader = command.ExecuteReader();
        var items = new List<XElement>();
        while (reader.Read()) items.Add(XElement.Parse(reader.GetString(0)));
        return items;
    }

    public void StoreElement(XElement element, string friendlyName)
    {
        using var connection = store.OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "INSERT INTO DataProtectionKeys(Xml) VALUES(@xml);";
        command.Parameters.AddWithValue("@xml", element.ToString(SaveOptions.DisableFormatting));
        command.ExecuteNonQuery();
    }
}
