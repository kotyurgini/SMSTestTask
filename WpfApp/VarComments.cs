using Newtonsoft.Json;
using System.IO;

namespace WpfApp;
public static class VarComments
{
    private static List<VarCommentItem> comments = [];
    private readonly static string pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "variableComments.json");

    public static void InitCommentFile()
    {
        if (!File.Exists(pathToFile))
            File.WriteAllText(pathToFile, "[]");
        else
            comments = JsonConvert.DeserializeObject<List<VarCommentItem>>(File.ReadAllText(pathToFile)) ?? [];
    }

    public static void SaveComments()
    {
        File.WriteAllText(pathToFile, JsonConvert.SerializeObject(comments, Formatting.Indented));
    }

    public static void SaveComment(string name, string comment)
    {
        var commentItem = comments.Find(x => x.VarName == name);
        if (commentItem == null)
        {
            commentItem = new VarCommentItem() { VarName = name };
            comments.Add(commentItem);
        }
        commentItem.Comment = comment;
        SaveComments();
    }

    public static string GetComment(string name)
        => comments.Find(x => x.VarName == name) is { } commentItem ? commentItem.Comment : "";
}

public class VarCommentItem
{
    public string VarName { get; set; } = "";
    public string Comment { get; set; } = "";
}
