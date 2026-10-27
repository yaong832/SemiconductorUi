using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>
    /// PM(Process Module) 상태를 표시하는 커스텀 컨트롤
    /// TableLayoutPanel 대신 직접 그리기로 깜빡임 방지
    /// 읽기 전용으로 사용자 입력 불가
    /// </summary>
    public class PmStatusPanel : Control
    {
        // 표시할 데이터
        private string _title = "PM";
        private string _statusText = "Idle";
        private string _recipeName = "-";
        private string _stepName = "-";
        private string _timeInfo = "0/0s";
        private string _waferInfo = "-";
        private string _envInfo = "";
        private string _stepMessage = "";
        private int _progressPercent = 0;
        private bool _isProcessing = false;
        private bool _hasAlarm = false;

        // 환경 정보 (PV/SV)
        private double _tempPV = 0, _tempSV = 0;
        private double _pressPV = 0, _pressSV = 0;
        private double _nf3PV = 0, _nf3SV = 0;
        private double _o2PV = 0, _o2SV = 0;
        private double _cf4PV = 0, _cf4SV = 0;
        private double _rfPV = 0, _rfSV = 0;

        // 색상 설정 (밝은 테마). 상태는 바탕색 대신 제목 옆 램프(대기/진행/알람)로 보여 줍니다.
        private readonly Color _backgroundColor = UiTheme.Surface;
        private readonly Color _borderColor = UiTheme.Border;
        private readonly Color _titleColor = UiTheme.TextPrimary;
        private readonly Color _labelColor = UiTheme.TextSecondary;
        private readonly Color _valueColor = UiTheme.TextPrimary;
        private readonly Color _readoutBack = UiTheme.SurfaceAlt;
        private readonly Color _pvActiveBack = Color.FromArgb(228, 243, 234);
        private readonly Color _pvActiveText = Color.FromArgb(28, 122, 73);
        private readonly Color _emptyText = Color.FromArgb(140, 150, 164);
        private readonly Color _progressBackColor = Color.FromArgb(230, 234, 240);

        // 폰트 (패널 크기에 맞춰 다시 만듭니다)
        private Font _titleFont;
        private Font _labelFont;
        private Font _lampFont;
        private Font _valueFont;
        private Font _stepMessageFont;
        private Font _readoutFont;
        private Font _readoutBoldFont;
        private float _fontScale;

        // 레이아웃 설정 (기준 크기, 패널 높이에 맞춰 늘리거나 줄입니다)
        private const int ContentPadding = 12;
        private const int TitleHeight = 24;
        private const int RowHeight = 22;
        private const int EnvHeaderHeight = 16;
        private const int StepMessageHeight = 18;
        private const int LabelWidth = 56;
        private const int ProgressBarHeight = 8;

        // 위 기준 크기로 그렸을 때의 전체 높이
        private const int DesignHeight =
            ContentPadding + TitleHeight + 4 + 8 + (RowHeight + 2) * 4 + 4 + 6
            + EnvHeaderHeight + RowHeight * 6 + 4 + StepMessageHeight + 2 + ProgressBarHeight + ContentPadding;

        private const float MinLayoutScale = 0.7f;
        private const float MaxLayoutScale = 1.4f;
        private const float MinFontScale = 0.85f;
        private const float MaxFontScale = 1.3f;

        // 현재 그리기 배율 (OnPaint에서 정함)
        private float _layoutScale = 1f;

        public PmStatusPanel()
        {
            // 깜빡임 방지를 위한 더블 버퍼링
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            BackColor = _backgroundColor;
            // 높이는 패널에 맞춰 줄여 그리므로 최소 높이를 두지 않습니다.
            MinimumSize = new Size(180, 0);
            EnsureFonts(1f);
        }

        #region Properties

        public string Title
        {
            get => _title;
            set { if (_title != value) { _title = value; Invalidate(); } }
        }

        public string StatusText
        {
            get => _statusText;
            set { if (_statusText != value) { _statusText = value; Invalidate(); } }
        }

        public string RecipeName
        {
            get => _recipeName;
            set { if (_recipeName != value) { _recipeName = value ?? "-"; Invalidate(); } }
        }

        public string StepName
        {
            get => _stepName;
            set { if (_stepName != value) { _stepName = value ?? "-"; Invalidate(); } }
        }

        public string TimeInfo
        {
            get => _timeInfo;
            set { if (_timeInfo != value) { _timeInfo = value ?? ""; Invalidate(); } }
        }

        public string WaferInfo
        {
            get => _waferInfo;
            set { if (_waferInfo != value) { _waferInfo = value ?? "-"; Invalidate(); } }
        }

        public string EnvInfo
        {
            get => _envInfo;
            set { if (_envInfo != value) { _envInfo = value ?? ""; Invalidate(); } }
        }

        public string StepMessage
        {
            get => _stepMessage;
            set { if (_stepMessage != value) { _stepMessage = value ?? ""; Invalidate(); } }
        }

        public int ProgressPercent
        {
            get => _progressPercent;
            set
            {
                var clamped = Math.Max(0, Math.Min(100, value));
                if (_progressPercent != clamped)
                {
                    _progressPercent = clamped;
                    Invalidate();
                }
            }
        }

        public bool IsProcessing
        {
            get => _isProcessing;
            set { if (_isProcessing != value) { _isProcessing = value; Invalidate(); } }
        }

        public bool HasAlarm
        {
            get => _hasAlarm;
            set { if (_hasAlarm != value) { _hasAlarm = value; Invalidate(); } }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 모든 값을 한 번에 업데이트 (여러 번 Invalidate 호출 방지)
        /// </summary>
        public void UpdateAllValues(string status, string recipe, string step, 
            string time, string wafer, string env, string stepMsg, int progress, bool processing)
        {
            _statusText = status ?? "";
            _recipeName = string.IsNullOrWhiteSpace(recipe) ? "-" : recipe;
            _stepName = string.IsNullOrWhiteSpace(step) ? "-" : step;
            _timeInfo = time ?? "";
            _waferInfo = string.IsNullOrWhiteSpace(wafer) ? "-" : wafer;
            _envInfo = env ?? "";
            _stepMessage = stepMsg ?? "";
            _progressPercent = Math.Max(0, Math.Min(100, progress));
            _isProcessing = processing;
            
            Invalidate(); // 한 번만 호출
        }

        /// <summary>
        /// 환경 정보 업데이트 (온도, 압력, NF3, O2, CF4, RF)
        /// </summary>
        public void UpdateEnvironmentValues(
            double tempPV, double tempSV,
            double pressPV, double pressSV,
            double nf3PV, double nf3SV,
            double o2PV, double o2SV,
            double cf4PV, double cf4SV,
            double rfPV, double rfSV)
        {
            _tempPV = tempPV;
            _tempSV = tempSV;
            _pressPV = pressPV;
            _pressSV = pressSV;
            _nf3PV = nf3PV;
            _nf3SV = nf3SV;
            _o2PV = o2PV;
            _o2SV = o2SV;
            _cf4PV = cf4PV;
            _cf4SV = cf4SV;
            _rfPV = rfPV;
            _rfSV = rfSV;
            
            Invalidate();
        }

        #endregion

        #region Painting

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var bounds = ClientRectangle;
            
            // 배경은 늘 흰색, 알람이면 테두리만 빨갛게 두껍게
            using (var bgBrush = new SolidBrush(_backgroundColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }
            using (var borderPen = new Pen(_hasAlarm ? UiTheme.Danger : _borderColor, _hasAlarm ? 2f : 1f))
            {
                g.DrawRectangle(borderPen, _hasAlarm ? 1 : 0, _hasAlarm ? 1 : 0, bounds.Width - (_hasAlarm ? 2 : 1), bounds.Height - (_hasAlarm ? 2 : 1));
            }

            _layoutScale = Math.Max(MinLayoutScale, Math.Min(MaxLayoutScale, bounds.Height / (float)DesignHeight));
            EnsureFonts(Math.Max(MinFontScale, Math.Min(MaxFontScale, _layoutScale)));

            int y = V(ContentPadding);

            // 제목 (PM 이름)
            DrawTitle(g, ref y, bounds.Width);

            // 구분선
            y += V(4);
            DrawSeparator(g, y, bounds.Width);
            y += V(8);

            // 정보 행들
            DrawInfoRow(g, "레시피", _recipeName, ref y, bounds.Width);
            DrawInfoRow(g, "단계", _stepName, ref y, bounds.Width);
            DrawInfoRow(g, "시간", _timeInfo, ref y, bounds.Width);
            DrawInfoRow(g, "웨이퍼", _waferInfo, ref y, bounds.Width);

            // 환경 정보 구분선
            y += V(4);
            DrawSeparator(g, y, bounds.Width);
            y += V(6);

            // 환경 정보 (NF3, O2, CF4, 압력, RF, 온도)
            DrawEnvHeader(g, ref y, bounds.Width);
            DrawEnvRow(g, "NF3", _nf3PV, _nf3SV, "sccm", ref y, bounds.Width);
            DrawEnvRow(g, "O2", _o2PV, _o2SV, "sccm", ref y, bounds.Width);
            DrawEnvRow(g, "CF4", _cf4PV, _cf4SV, "sccm", ref y, bounds.Width);
            DrawEnvRow(g, "압력", _pressPV, _pressSV, "Torr", ref y, bounds.Width);
            DrawEnvRow(g, "RF", _rfPV, _rfSV, "W", ref y, bounds.Width);
            DrawEnvRow(g, "온도", _tempPV, _tempSV, "°C", ref y, bounds.Width);

            // 진행률 바 (상태 메시지와 함께)
            y += V(4);
            DrawProgressBar(g, ref y, bounds.Width);
        }

        /// <summary>기준 크기의 세로 길이를 현재 배율로 바꿉니다.</summary>
        private int V(int designValue)
        {
            return (int)Math.Round(designValue * _layoutScale);
        }

        /// <summary>라벨 칸 폭. 글자 크기에 맞춰 늘어납니다.</summary>
        private int ScaledLabelWidth => (int)Math.Round(LabelWidth * _fontScale);

        private void EnsureFonts(float scale)
        {
            scale = (float)Math.Round(scale, 2);
            if (_titleFont != null && Math.Abs(scale - _fontScale) < 0.01f) return;

            DisposeFonts();
            _fontScale = scale;
            _titleFont = new Font("Segoe UI", 10f * scale, FontStyle.Bold);
            _labelFont = new Font("Segoe UI", 8f * scale, FontStyle.Bold);
            _lampFont = new Font("Segoe UI", 9f * scale, FontStyle.Bold);
            _valueFont = new Font("Segoe UI", 9f * scale, FontStyle.Regular);
            _stepMessageFont = new Font("Segoe UI", 9f * scale, FontStyle.Bold);
            _readoutFont = new Font("Consolas", 9.5f * scale, FontStyle.Regular);
            _readoutBoldFont = new Font("Consolas", 9.5f * scale, FontStyle.Bold);
        }

        private void DisposeFonts()
        {
            _titleFont?.Dispose();
            _labelFont?.Dispose();
            _lampFont?.Dispose();
            _valueFont?.Dispose();
            _stepMessageFont?.Dispose();
            _readoutFont?.Dispose();
            _readoutBoldFont?.Dispose();
        }

        /// <summary>값 표시창: 둥근 연회색 칸.</summary>
        private static void FillReadout(Graphics g, Rectangle rect, Color back)
        {
            using (var path = RoundedRect(rect, 4))
            using (var brush = new SolidBrush(back))
            {
                g.FillPath(brush, path);
            }
        }

        private static GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            var path = new GraphicsPath();
            int d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
            if (d <= 0)
            {
                path.AddRectangle(rect);
                return path;
            }
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void DrawTitle(Graphics g, ref int y, int width)
        {
            var titleRect = new Rectangle(ContentPadding, y, width - ContentPadding * 2, V(TitleHeight));
            
            TextRenderer.DrawText(g, _title, _titleFont, titleRect, _titleColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            // 상태 램프 (우측): 대기 / 진행 / 알람 중 하나가 켜집니다.
            var lamps = new[]
            {
                Tuple.Create("알람", LampTone.Red, _hasAlarm),
                Tuple.Create("진행", LampTone.Green, !_hasAlarm && _isProcessing),
                Tuple.Create("대기", LampTone.Amber, !_hasAlarm && !_isProcessing)
            };
            int right = titleRect.Right;
            int lampSize = Math.Max(10, (int)Math.Round(13 * _fontScale));
            int centerY = titleRect.Y + titleRect.Height / 2;
            foreach (var lamp in lamps)
            {
                var textSize = TextRenderer.MeasureText(g, lamp.Item1, _lampFont, Size.Empty, TextFormatFlags.NoPadding);
                int textX = right - textSize.Width;
                TextRenderer.DrawText(g, lamp.Item1, _lampFont, new Point(textX, centerY - textSize.Height / 2),
                    lamp.Item3 ? _titleColor : _emptyText, TextFormatFlags.NoPadding);
                var lampRect = new RectangleF(textX - 6 - lampSize, centerY - lampSize / 2f, lampSize, lampSize);
                LampButtonPainter.DrawLamp(g, lampRect, lamp.Item3 ? lamp.Item2 : LampTone.Off);
                right = (int)lampRect.Left - 14;
            }

            y += V(TitleHeight);
        }

        private void DrawSeparator(Graphics g, int y, int width)
        {
            using (var pen = new Pen(Color.FromArgb(200, 200, 210), 1))
            {
                g.DrawLine(pen, ContentPadding, y, width - ContentPadding, y);
            }
        }

        private void DrawInfoRow(Graphics g, string label, string value, ref int y, int width)
        {
            int rowHeight = V(RowHeight);
            var labelRect = new Rectangle(ContentPadding, y, ScaledLabelWidth, rowHeight);
            var valueRect = new Rectangle(ContentPadding + ScaledLabelWidth + 4, y, width - ContentPadding * 2 - ScaledLabelWidth - 4, rowHeight);

            // 라벨은 글자만, 값은 표시창 안에 고정폭 글꼴로
            TextRenderer.DrawText(g, label, _labelFont, labelRect, _labelColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
            FillReadout(g, valueRect, _readoutBack);
            var valueTextRect = Rectangle.Inflate(valueRect, -8, 0);
            TextRenderer.DrawText(g, value, _readoutFont, valueTextRect, _valueColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);

            y += rowHeight + V(2);
        }

        /// <summary>
        /// 환경 값 열 위치. 라벨 칸 뒤의 남은 폭을 현재값/설정값 두 칸이 반씩 나눠 씁니다.
        /// </summary>
        private void GetEnvColumns(int width, out int labelWidth, out int pvX, out int svX, out int valueWidth)
        {
            const int gap = 4;
            labelWidth = ScaledLabelWidth;
            pvX = ContentPadding + labelWidth + gap;
            int remaining = width - ContentPadding - pvX;
            valueWidth = Math.Max(20, (remaining - gap) / 2);
            svX = pvX + valueWidth + gap;
        }

        private void DrawEnvHeader(Graphics g, ref int y, int width)
        {
            GetEnvColumns(width, out _, out int pvX, out int svX, out int valueWidth);
            int height = V(EnvHeaderHeight);
            var flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;

            TextRenderer.DrawText(g, "현재값 PV", _labelFont, new Rectangle(pvX, y, valueWidth, height), _pvActiveText, flags);
            TextRenderer.DrawText(g, "설정값 SV", _labelFont, new Rectangle(svX, y, valueWidth, height), UiTheme.Primary, flags);

            y += height;
        }

        private void DrawEnvRow(Graphics g, string label, double pv, double sv, string unit, ref int y, int width)
        {
            GetEnvColumns(width, out int labelWidth, out int pvX, out int svX, out int valueWidth);
            int rowHeight = V(RowHeight);
            var labelRect = new Rectangle(ContentPadding, y, labelWidth, rowHeight - V(2));
            var pvRect = new Rectangle(pvX, y, valueWidth, rowHeight - V(2));
            var svRect = new Rectangle(svX, y, valueWidth, rowHeight - V(2));

            bool hasPv = _isProcessing && pv > 0;

            // 라벨
            TextRenderer.DrawText(g, label, _labelFont, labelRect, UiTheme.TextPrimary,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            // PV: 공정 중 값이 있으면 연한 초록 표시창
            FillReadout(g, pvRect, hasPv ? _pvActiveBack : _readoutBack);
            string pvText = hasPv ? WithUnit(g, FormatValue(pv, unit), unit, pvRect.Width) : "-";
            TextRenderer.DrawText(g, pvText, hasPv ? _readoutBoldFont : _readoutFont, Rectangle.Inflate(pvRect, -8, 0),
                hasPv ? _pvActiveText : _emptyText,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            // SV: 설정값이 있으면 파란 글자
            FillReadout(g, svRect, _readoutBack);
            string svText = (sv > 0) ? WithUnit(g, FormatValue(sv, unit), unit, svRect.Width) : "-";
            TextRenderer.DrawText(g, svText, _readoutFont, Rectangle.Inflate(svRect, -8, 0),
                sv > 0 ? UiTheme.Primary : _emptyText,
                TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);

            y += rowHeight;
        }

        /// <summary>칸이 넉넉하면 값 뒤에 단위를 붙입니다.</summary>
        private string WithUnit(Graphics g, string value, string unit, int cellWidth)
        {
            if (value == "-" || string.IsNullOrEmpty(unit)) return value;

            string withUnit = value + " " + unit;
            var size = TextRenderer.MeasureText(g, withUnit, _readoutBoldFont, Size.Empty, TextFormatFlags.NoPadding);
            return size.Width + 16 <= cellWidth ? withUnit : value;
        }

        private string FormatValue(double value, string unit)
        {
            if (value == 0) return "-";
            
            // 단위에 따른 포맷
            switch (unit)
            {
                case "°C":
                    return $"{value:F1}";
                case "Torr":
                    return value < 1.0 ? $"{value:F3}" : $"{value:F1}";
                case "sccm":
                    return $"{value:F0}";
                case "W":
                    return $"{value:F0}";
                default:
                    return $"{value:F2}";
            }
        }

        private void DrawProgressBar(Graphics g, ref int y, int width)
        {
            // 상태 메시지 줄을 진행률 바 위에 따로 둡니다 (환경 표와 겹치지 않게)
            int msgHeight = V(StepMessageHeight);
            var msgRect = new Rectangle(ContentPadding, y, width - ContentPadding * 2, msgHeight);
            y += msgHeight + V(2);

            var barRect = new Rectangle(ContentPadding, y, width - ContentPadding * 2, Math.Max(4, V(ProgressBarHeight)));

            // 배경 홈과 채움 (알람이면 빨강)
            using (var path = RoundedRect(barRect, barRect.Height / 2))
            using (var bgBrush = new SolidBrush(_progressBackColor))
            {
                g.FillPath(bgBrush, path);
            }
            if (_progressPercent > 0)
            {
                var fillWidth = Math.Max(barRect.Height, (int)(barRect.Width * (_progressPercent / 100.0)));
                var fillRect = new Rectangle(barRect.X, barRect.Y, fillWidth, barRect.Height);
                using (var path = RoundedRect(fillRect, barRect.Height / 2))
                using (var fillBrush = new SolidBrush(_hasAlarm ? UiTheme.Danger : UiTheme.Success))
                {
                    g.FillPath(fillBrush, path);
                }
            }

            // 상태 메시지를 프로그레스바 위에 표시 (우측 정렬)
            if (!string.IsNullOrWhiteSpace(_stepMessage))
            {
                TextRenderer.DrawText(g, _stepMessage, _stepMessageFont, msgRect, _valueColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            y += barRect.Height;
        }

        private void DrawStepMessage(Graphics g, ref int y, int width)
        {
            if (string.IsNullOrWhiteSpace(_stepMessage)) return;

            var msgRect = new Rectangle(ContentPadding, y, width - ContentPadding * 2, 24);
            TextRenderer.DrawText(g, _stepMessage, _stepMessageFont, msgRect, _valueColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            y += 24;
        }

        #endregion

        #region Dispose

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeFonts();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}

