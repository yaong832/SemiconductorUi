using System;
using System.IO;
using System.Reflection;
using System.Text;
using IEG3268_Dll;

class Program
{
    static void Main()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== IEG3268 DLL 메서드 목록 ===");
        sb.AppendLine("");

        try
        {
            Type type = typeof(IEG3268);
            sb.AppendLine($"클래스: {type.FullName}");
            sb.AppendLine($"네임스페이스: {type.Namespace}");
            sb.AppendLine($"어셈블리: {type.Assembly.FullName}");
            sb.AppendLine("");

            // Public 메서드 목록
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Array.Sort(methods, (a, b) => string.Compare(a.Name, b.Name));

            sb.AppendLine($"총 {methods.Length}개의 public 메서드:");
            sb.AppendLine("");

            foreach (var method in methods)
            {
                var parameters = method.GetParameters();
                var paramList = string.Join(", ", Array.ConvertAll(parameters, p => $"{p.ParameterType.Name} {p.Name}"));
                sb.AppendLine($"  {method.ReturnType.Name} {method.Name}({paramList})");
            }

            // Digital 관련 메서드 특별 표시
            sb.AppendLine("");
            sb.AppendLine("=== Digital 관련 메서드 ===");
            sb.AppendLine("");

            bool foundDigital = false;
            foreach (var method in methods)
            {
                if (method.Name.Contains("Digital", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("Input", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("Output", StringComparison.OrdinalIgnoreCase))
                {
                    foundDigital = true;
                    var parameters = method.GetParameters();
                    var paramList = string.Join(", ", Array.ConvertAll(parameters, p => $"{p.ParameterType.Name} {p.Name}"));
                    sb.AppendLine($"  {method.ReturnType.Name} {method.Name}({paramList})");
                }
            }

            if (!foundDigital)
            {
                sb.AppendLine("  Digital 관련 메서드를 찾을 수 없습니다.");
            }

            // Properties 목록
            sb.AppendLine("");
            sb.AppendLine("=== Properties ===");
            sb.AppendLine("");

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            Array.Sort(properties, (a, b) => string.Compare(a.Name, b.Name));

            foreach (var prop in properties)
            {
                string getSet = prop.CanWrite ? "get; set;" : "get;";
                sb.AppendLine($"  {prop.PropertyType.Name} {prop.Name} {{ {getSet} }}");
            }

            // 파일로 저장
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "IEG3268_Methods.txt");
            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);

            Console.WriteLine(sb.ToString());
            Console.WriteLine($"\n메서드 목록이 저장되었습니다: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"오류 발생: {ex.Message}");
            Console.WriteLine($"상세: {ex}");
        }

        Console.WriteLine("\n아무 키나 누르면 종료합니다...");
        Console.ReadKey();
    }
}
