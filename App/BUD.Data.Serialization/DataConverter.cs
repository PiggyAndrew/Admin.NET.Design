using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSD.Data.Serialization;

public class DataConverter
{
    /// <summary>
    /// Asynchronously deserialize a json file into <see cref="DSD_Project"/>
    /// </summary>
    /// <param name="filePath">file to be deserialize</param>
    /// <returns><see cref="DSD_Project"/> object</returns>
    public static async Task<DSD_Project> DeserializeAsync(string filePath)
    {
        using TextReader textReader = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
        string jsonData = await textReader.ReadToEndAsync();

        DSD_Project? project = JsonConvert.DeserializeObject<DSD_Project>(jsonData, new JsonSerializerSettings() { });

        return project ?? throw new Exception("Deserialize failed");
    }

    /// <summary>
    /// Asynchronously serialize <see cref="DSD_Project"/> to a file
    /// </summary>
    /// <param name="project"></param>
    /// <param name="path">file to save data</param>
    public static async Task SerializeAsync(DSD_Project project, string path)
    {
        DirectoryInfo fileParent = Directory.GetParent(path);
        if (Path.GetExtension(path).ToLower() != "json")
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            path = $"{fileParent.FullName}\\{fileName}.json";
        }

        if (!fileParent.Exists)
        {
            fileParent.Create();
        }

        string json = JsonConvert.SerializeObject(project, new JsonSerializerSettings() { });


        using TextWriter textWriter = new StreamWriter(File.Create(path));
        await textWriter.WriteAsync(json);
        await textWriter.FlushAsync();

        textWriter.Close();
    }
}
