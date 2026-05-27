using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

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

        // 색상 설정
        private readonly Color _backgroundColor = Color.FromArgb(250, 250, 255);
        private readonly Color _borderColor = Color.FromArgb(220, 220, 230);
        private readonly Color _titleColor = Color.FromArgb(40, 40, 40);
        private readonly Color _labelColor = Color.FromArgb(60, 60, 60);
        private readonly Color _valueColor = Color.FromArgb(40, 40, 40);
        private readonly Color _progressBackColor = Color.FromArgb(230, 230, 240);
        private readonly Color _progressForeColor = Color.FromArgb(76, 175, 80);
        private readonly Color _processingColor = Color.FromArgb(200, 230, 210);
        private readonly Color _idleColor = Color.FromArgb(250, 250, 255);
        private readonly Color _alarmColor = Color.FromArgb(255, 200, 200); // 알람 배경색 (연한 빨강)

        // 폰트
        private readonly Font _titleFont = new Font("Segoe UI", 10f, FontStyle.Bold);
        private readonly Font _labelFont = new Font("Segoe UI", 8f, FontStyle.Bold);
        private readonly Font _valueFont = new Font("Segoe UI", 9f, FontStyle.Regular);
        private readonly Font _stepMessageFont = new Font("Segoe UI", 9f, FontStyle.Bold);

        // 레이아웃 설정
        private const int ContentPadding = 12;
        private const int RowHeight = 22;
        private const int LabelWidth = 56;
        private const int ProgressBarHeight = 8;

        public PmStatusPanel()
        {
            // 깜빡임 방지를 위한 더블 버퍼링
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.ResizeRedraw, true);
            UpdateStyles();

            BackColor = _backgroundColor;
            MinimumSize = new Size(180, 300); // 상태 메시지가 프로그레스바 위에 표시되므로 높이 감소
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
            
            // 배경 (알람 상태에 따라 색상 변경)
            Color bgColor;
            if (_hasAlarm)
            {
                bgColor = _alarmColor;
            }
            else if (_isProcessing)
            {
                bgColor = _processingColor;
            }
            else
            {
                bgColor = _backgroundColor;
            }
            
            using (var bgBrush = new SolidBrush(bgColor))
            {
                g.FillRectangle(bgBrush, bounds);
            }

            // 테두리
            using (var borderPen = new Pen(_borderColor, 1))
            {
                g.DrawRectangle(borderPen, 0, 0, bounds.Width - 1, bounds.Height - 1);
            }

            int y = ContentPadding;

            // 제목 (PM 이름)
            DrawTitle(g, ref y, bounds.Width);

            // 구분선
            y += 4;
            DrawSeparator(g, y, bounds.Width);
            y += 8;

            // 정보 행들
            DrawInfoRow(g, "레시피", _recipeName, ref y, bounds.Width);
            DrawInfoRow(g, "단계", _stepName, ref y, bounds.Width);
            DrawInfoRow(g, "시간", _timeInfo, ref y, bounds.Width);
            DrawInfoRow(g, "웨이퍼", _waferInfo, ref y, bounds.Width);

            // 환경 정보 구분선
            y += 4;
            DrawSeparator(g, y, bounds.Width);
            y += 6;

            // 환경 정보 (NF3, O2, CF4, 압력, RF, 온도)
            DrawEnvRow(g, "NF3", _nf3PV, _nf3SV, "sccm", ref y, bounds.Width);
            DrawEnvRow(g, "O2", _o2PV, _o2SV, "sccm", ref y, bounds.Width);
            DrawEnvRow(g, "CF4", _cf4PV, _cf4SV, "sccm", ref y, bounds.Width);
            DrawEnvRow(g, "압력", _pressPV, _pressSV, "Torr", ref y, bounds.Width);
            DrawEnvRow(g, "RF", _rfPV, _rfSV, "W", ref y, bounds.Width);
            DrawEnvRow(g, "온도", _tempPV, _tempSV, "°C", ref y, bounds.Width);

            // 진행률 바 (상태 메시지와 함께)
            y += 6;
            DrawProgressBar(g, ref y, bounds.Width);
        }

        private void DrawTitle(Graphics g, ref int y, int width)
        {
            var titleRect = new Rectangle(ContentPadding, y, width - ContentPadding * 2, 24);
            
            TextRenderer.DrawText(g, _title, _titleFont, titleRect, _titleColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            // 상태 텍스트 (우측)
            if (!string.IsNullOrWhiteSpace(_statusText))
            {
                var statusColor = _isProcessing ? Color.FromArgb(76, 175, 80) : Color.FromArgb(100, 100, 100);
                TextRenderer.DrawText(g, _statusText, _valueFont, titleRect, statusColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter);
            }

            y += 24;
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
            var labelRect = new Rectangle(ContentPadding, y, LabelWidth, RowHeight);
            var valueRect = new Rectangle(ContentPadding + LabelWidth + 4, y, width - ContentPadding * 2 - LabelWidth - 4, RowHeight);

            // 라벨 배경 (밝은 회색)
            using (var labelBgBrush = new SolidBrush(Color.FromArgb(235, 235, 245)))
            {
                g.FillRectangle(labelBgBrush, labelRect);
            }

            // 라벨 테두리
            using (var borderPen = new Pen(Color.FromArgb(200, 200, 210), 1))
            {
                g.DrawRectangle(borderPen, labelRect);
                g.DrawRectangle(borderPen, valueRect);
            }

            // 텍스트
            TextRenderer.DrawText(g, label, _labelFont, labelRect, _labelColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            TextRenderer.DrawText(g, value, _valueFont, valueRect, _valueColor,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            y += RowHeight + 2;
        }

        private void DrawEnvRow(Graphics g, string label, double pv, double sv, string unit, ref int y, int width)
        {
            var labelRect = new Rectangle(ContentPadding, y, 40, RowHeight - 2);
            var pvRect = new Rectangle(ContentPadding + 44, y, 58, RowHeight - 2);
            var svRect = new Rectangle(ContentPadding + 106, y, 58, RowHeight - 2);

            // 라벨 배경
            using (var labelBgBrush = new SolidBrush(Color.FromArgb(235, 235, 245)))
            {
                g.FillRectangle(labelBgBrush, labelRect);
            }

            // PV 배경 (공정 중이고 값이 있으면 약간 밝게)
            var pvBgColor = (_isProcessing && pv > 0) ? Color.FromArgb(240, 245, 250) : Color.FromArgb(245, 245, 250);
            using (var pvBgBrush = new SolidBrush(pvBgColor))
            {
                g.FillRectangle(pvBgBrush, pvRect);
            }

            // SV 배경
            using (var svBgBrush = new SolidBrush(Color.FromArgb(245, 245, 250)))
            {
                g.FillRectangle(svBgBrush, svRect);
            }

            // 테두리
            using (var borderPen = new Pen(Color.FromArgb(200, 200, 210), 1))
            {
                g.DrawRectangle(borderPen, labelRect);
                g.DrawRectangle(borderPen, pvRect);
                g.DrawRectangle(borderPen, svRect);
            }

            // 라벨 텍스트
            TextRenderer.DrawText(g, label, _labelFont, labelRect, _labelColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // PV 값 (공정 중이면 값 표시, 아니면 "-")
            string pvText = (_isProcessing && pv > 0) ? FormatValue(pv, unit) : "-";
            var pvColor = (_isProcessing && pv > 0) ? Color.FromArgb(76, 175, 80) : Color.FromArgb(100, 100, 100);
            TextRenderer.DrawText(g, pvText, _valueFont, pvRect, pvColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            // SV 값 (설정값이 있으면 표시)
            string svText = (sv > 0) ? FormatValue(sv, unit) : "-";
            TextRenderer.DrawText(g, svText, _valueFont, svRect, Color.FromArgb(33, 150, 243),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            y += RowHeight;
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
            var barRect = new Rectangle(ContentPadding, y, width - ContentPadding * 2, ProgressBarHeight);

            // 배경
            using (var bgBrush = new SolidBrush(_progressBackColor))
            {
                g.FillRectangle(bgBrush, barRect);
            }

            // 진행률
            if (_progressPercent > 0)
            {
                var fillWidth = (int)((barRect.Width - 2) * (_progressPercent / 100.0));
                var fillRect = new Rectangle(barRect.X + 1, barRect.Y + 1, fillWidth, barRect.Height - 2);
                
                using (var fillBrush = new SolidBrush(_progressForeColor))
                {
                    g.FillRectangle(fillBrush, fillRect);
                }
            }

            // 테두리
            using (var borderPen = new Pen(Color.FromArgb(200, 200, 210), 1))
            {
                g.DrawRectangle(borderPen, barRect);
            }

            // 상태 메시지를 프로그레스바 위에 표시 (겹치지 않게)
            if (!string.IsNullOrWhiteSpace(_stepMessage))
            {
                // 프로그레스바 위쪽에 메시지 표시 (우측 정렬, 겹치지 않게 여유 공간 확보)
                var msgY = y - 20; // 프로그레스바 위 20px (메시지 높이 고려)
                var msgRect = new Rectangle(ContentPadding, msgY, width - ContentPadding * 2, 18);
                TextRenderer.DrawText(g, _stepMessage, _stepMessageFont, msgRect, _valueColor,
                    TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            }

            y += ProgressBarHeight;
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
                _titleFont?.Dispose();
                _labelFont?.Dispose();
                _valueFont?.Dispose();
                _stepMessageFont?.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}

