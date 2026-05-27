using System;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using IEG3268_Dll;

namespace SemiconductorUi
{
    /// <summary>
    /// IEG3268 DLL의 메서드 목록을 확인하는 유틸리티 클래스
    /// </summary>
    public static class DllMethodInspector
    {
        /// <summary>
        /// IEG3268 클래스의 모든 public 메서드를 나열합니다
        /// </summary>
        /// <returns>메서드 목록 문자열</returns>
        public static string ListAllMethods()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== IEG3268 DLL 메서드 목록 ===\n");

            try
            {
                Type type = typeof(IEG3268);
                sb.AppendLine($"클래스 이름: {type.FullName}");
                sb.AppendLine($"네임스페이스: {type.Namespace}");
                sb.AppendLine($"어셈블리: {type.Assembly.FullName}\n");

                // Public 메서드 목록
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .OrderBy(m => m.Name)
                    .ToList();

                sb.AppendLine($"총 {methods.Count}개의 public 메서드:\n");

                foreach (var method in methods)
                {
                    // 매개변수 목록
                    var parameters = method.GetParameters();
                    var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));

                    sb.AppendLine($"  {method.ReturnType.Name} {method.Name}({paramList})");
                }

                // Timer 관련 메서드 특별 표시
                sb.AppendLine("\n\n=== Timer 관련 메서드 ===\n");
                var timerMethods = methods.Where(m => 
                    m.Name.IndexOf("Timer", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.Name.IndexOf("ReadData", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.Name.IndexOf("Stop", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    m.Name.IndexOf("Start", StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                if (timerMethods.Any())
                {
                    foreach (var method in timerMethods)
                    {
                        var parameters = method.GetParameters();
                        var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        sb.AppendLine($"  {method.ReturnType.Name} {method.Name}({paramList})");
                    }
                }
                else
                {
                    sb.AppendLine("  Timer 관련 메서드를 찾을 수 없습니다.");
                }

                // Properties 목록
                sb.AppendLine("\n\n=== Properties ===\n");
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .OrderBy(p => p.Name)
                    .ToList();

                foreach (var prop in properties)
                {
                    sb.AppendLine($"  {prop.PropertyType.Name} {prop.Name} {{ get; {(prop.CanWrite ? "set; " : "")}}}");
                }

            }
            catch (Exception ex)
            {
                sb.AppendLine($"\n오류 발생: {ex.Message}");
                sb.AppendLine($"상세: {ex}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// 메서드 목록을 메시지박스로 표시합니다 (디버깅용)
        /// </summary>
        public static void ShowMethodsInMessageBox()
        {
            string methods = ListAllMethods();
            MessageBox.Show(methods, "IEG3268 DLL 메서드 목록", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 메서드 목록을 파일로 저장합니다
        /// </summary>
        /// <param name="filePath">저장할 파일 경로</param>
        public static void SaveMethodsToFile(string filePath)
        {
            try
            {
                string methods = ListAllMethods();
                System.IO.File.WriteAllText(filePath, methods, Encoding.UTF8);
                MessageBox.Show($"메서드 목록이 저장되었습니다:\n{filePath}", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 저장 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 특정 이름 패턴과 일치하는 메서드를 검색합니다
        /// </summary>
        /// <param name="pattern">검색할 패턴 (대소문자 무시)</param>
        /// <returns>메서드 목록 문자열</returns>
        public static string SearchMethods(string pattern)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"=== '{pattern}' 패턴으로 검색된 메서드 ===\n");

            try
            {
                Type type = typeof(IEG3268);
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Where(m => m.Name.IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderBy(m => m.Name)
                    .ToList();

                if (methods.Any())
                {
                    foreach (var method in methods)
                    {
                        var parameters = method.GetParameters();
                        var paramList = string.Join(", ", parameters.Select(p => $"{p.ParameterType.Name} {p.Name}"));
                        sb.AppendLine($"  {method.ReturnType.Name} {method.Name}({paramList})");
                    }
                    sb.AppendLine($"\n총 {methods.Count}개의 메서드를 찾았습니다.");
                }
                else
                {
                    sb.AppendLine($"'{pattern}' 패턴과 일치하는 메서드를 찾을 수 없습니다.");
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"\n오류 발생: {ex.Message}");
            }

            return sb.ToString();
        }
    }
}

