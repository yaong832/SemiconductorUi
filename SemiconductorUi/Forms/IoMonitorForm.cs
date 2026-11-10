using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using SemiconductorUi.Hardware;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// I/O 모니터: 디지털 입력·출력 16점씩과 두 서보 축의 상태를 이름과 함께 보여 줍니다.
    /// 읽기 전용입니다. 장비에 아무 명령도 보내지 않고, DLL이 0.3초마다 받아 둔 값을 읽기만 합니다.
    /// 메인 화면을 잠그지 않으므로(모달 아님), 떠 있는 동안에도 긴급 정지를 그대로 누를 수 있습니다.
    /// 출력은 DLL로 단자 상태를 다시 읽을 수 없어서 "마지막으로 보낸 값"을 보여 줍니다.
    /// </summary>
    internal sealed class IoMonitorForm : Form
    {
        internal const int PointCount = 16;
        private const int RefreshIntervalMs = 300;

        /// <summary>입력 번호별 이름. 코드에서 쓰지 않는 번호는 "미사용"으로 둡니다.</summary>
        internal static readonly string[] InputNames = BuildInputNames();

        /// <summary>출력 번호별 이름</summary>
        internal static readonly string[] OutputNames =
        {
            "적색 램프",
            "황색 램프",
            "녹색 램프",
            "Chamber A 램프",
            "Chamber A 도어 닫힘 (상승 SOL)",
            "Chamber A 도어 열림 (하강 SOL)",
            "Chamber B 램프",
            "Chamber B 도어 닫힘 (상승 SOL)",
            "Chamber B 도어 열림 (하강 SOL)",
            "Chamber C 램프",
            "Chamber C 도어 닫힘 (상승 SOL)",
            "Chamber C 도어 열림 (하강 SOL)",
            "실린더 전진",
            "실린더 후진",
            "진공 (흡착)",
            "배기"
        };

        private static string[] BuildInputNames()
        {
            var names = new string[PointCount];
            for (int i = 0; i < PointCount; i++)
            {
                names[i] = "미사용";
            }
            names[12] = "실린더 후진 센서";
            names[13] = "실린더 전진 센서";
            return names;
        }

        private readonly Func<IEthercatDevice> deviceProvider;
        private readonly Func<bool> connectedProvider;
        private readonly Timer refreshTimer;
        private readonly Label labelStatus;
        private readonly IoPointPanel panelInputs;
        private readonly IoPointPanel panelOutputs;
        private readonly ServoStatusPanel panelServo;

        public IoMonitorForm(Func<IEthercatDevice> deviceProvider, Func<bool> connectedProvider)
        {
            this.deviceProvider = deviceProvider ?? throw new ArgumentNullException(nameof(deviceProvider));
            this.connectedProvider = connectedProvider ?? throw new ArgumentNullException(nameof(connectedProvider));

            Text = "I/O 모니터 (읽기 전용)";
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            BackColor = UiTheme.Surface;
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            ClientSize = new Size(900, 640);
            MinimumSize = new Size(720, 560);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(12),
                BackColor = UiTheme.Surface
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));

            labelStatus = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };
            layout.Controls.Add(labelStatus, 0, 0);
            layout.SetColumnSpan(labelStatus, 2);

            panelInputs = new IoPointPanel("디지털 입력 (센서)", InputNames) { Dock = DockStyle.Fill, Margin = new Padding(0, 4, 6, 4) };
            panelOutputs = new IoPointPanel("디지털 출력 (마지막으로 보낸 값)", OutputNames) { Dock = DockStyle.Fill, Margin = new Padding(6, 4, 0, 4) };
            layout.Controls.Add(panelInputs, 0, 1);
            layout.Controls.Add(panelOutputs, 1, 1);

            panelServo = new ServoStatusPanel { Dock = DockStyle.Fill, Margin = new Padding(0, 4, 0, 4) };
            layout.Controls.Add(panelServo, 0, 2);
            layout.SetColumnSpan(panelServo, 2);

            var labelNote = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = UiTheme.TextSecondary,
                TextAlign = ContentAlignment.MiddleLeft,
                Text = "보기 전용 · 0.3초마다 갱신 · 출력 상태는 실제 단자가 아니라 프로그램이 마지막으로 보낸 값입니다."
            };
            layout.Controls.Add(labelNote, 0, 3);
            layout.SetColumnSpan(labelNote, 2);

            Controls.Add(layout);

            refreshTimer = new Timer { Interval = RefreshIntervalMs };
            refreshTimer.Tick += (s, e) => RefreshValues();
            VisibleChanged += (s, e) =>
            {
                if (Visible)
                {
                    RefreshValues();
                    refreshTimer.Start();
                }
                else
                {
                    refreshTimer.Stop();
                }
            };
        }

        /// <summary>
        /// 값을 한 번 읽어 화면에 반영합니다.
        /// </summary>
        public void RefreshValues()
        {
            var device = deviceProvider();
            bool connected = device != null && connectedProvider();
            if (!connected)
            {
                labelStatus.Text = "EtherCAT 미연결 · 값을 읽을 수 없습니다";
                labelStatus.ForeColor = UiTheme.TextSecondary;
                panelInputs.SetValues(null);
                panelOutputs.SetValues(null);
                panelServo.SetValues(null, null);
                return;
            }

            bool[] inputs = new bool[PointCount];
            bool[] outputs = new bool[PointCount];
            ServoAxisSnapshot axis1;
            ServoAxisSnapshot axis2;
            try
            {
                for (int i = 0; i < PointCount; i++)
                {
                    inputs[i] = device.Digital_Input(i);
                    outputs[i] = device.GetDigitalOutput(i);
                }
                axis1 = ServoAxisSnapshot.Read(device, 1);
                axis2 = ServoAxisSnapshot.Read(device, 2);
            }
            catch (Exception ex)
            {
                labelStatus.Text = $"읽기 오류: {ex.Message}";
                labelStatus.ForeColor = UiTheme.Danger;
                return;
            }

            bool stopped = device.IsEmergencyStopped;
            labelStatus.Text = $"EtherCAT 연결됨{(stopped ? " · 비상정지 중" : "")} · {DateTime.Now:HH:mm:ss.f} 갱신";
            labelStatus.ForeColor = stopped ? UiTheme.Danger : UiTheme.Success;
            panelInputs.SetValues(inputs);
            panelOutputs.SetValues(outputs);
            panelServo.SetValues(axis1, axis2);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                Hide();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 사용자가 X로 닫으면 숨기기만 하고 다음에 다시 씁니다.
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
                return;
            }

            refreshTimer.Stop();
            base.OnFormClosing(e);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// 그린 점(ON/OFF)으로 16점의 상태를 보여 줍니다. 값이 바뀔 때만 다시 그립니다.
        /// </summary>
        private sealed class IoPointPanel : Control
        {
            private static readonly Font TitleFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            private static readonly Font RowFont = new Font("Segoe UI", 9F);
            private static readonly Color OnColor = UiTheme.Success;
            private static readonly Color OffColor = Color.FromArgb(200, 206, 214);

            private readonly string title;
            private readonly string[] names;
            private bool[] values;

            public IoPointPanel(string title, string[] names)
            {
                this.title = title;
                this.names = names;
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                    | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                BackColor = UiTheme.Surface;
            }

            public void SetValues(bool[] newValues)
            {
                if (SameValues(values, newValues))
                {
                    return;
                }
                values = newValues == null ? null : (bool[])newValues.Clone();
                Invalidate();
            }

            private static bool SameValues(bool[] a, bool[] b)
            {
                if (a == null || b == null)
                {
                    return a == null && b == null;
                }
                if (a.Length != b.Length)
                {
                    return false;
                }
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i] != b[i]) return false;
                }
                return true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;

                using (var pen = new Pen(UiTheme.Border))
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
                TextRenderer.DrawText(g, title, TitleFont, new Rectangle(10, 6, Width - 20, 22), UiTheme.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                int top = 32;
                int rowHeight = Math.Max(18, (Height - top - 6) / Math.Max(1, names.Length));
                for (int i = 0; i < names.Length; i++)
                {
                    int y = top + i * rowHeight;
                    if (i % 2 == 1)
                    {
                        using (var brush = new SolidBrush(UiTheme.SurfaceAlt))
                        {
                            g.FillRectangle(brush, 1, y, Width - 2, rowHeight);
                        }
                    }

                    bool unused = names[i] == "미사용";
                    Color textColor = unused ? UiTheme.TextSecondary : UiTheme.TextPrimary;
                    TextRenderer.DrawText(g, i.ToString("00"), RowFont, new Rectangle(10, y, 30, rowHeight), UiTheme.TextSecondary,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                    TextRenderer.DrawText(g, names[i], RowFont, new Rectangle(42, y, Width - 120, rowHeight), textColor,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

                    string stateText;
                    Color dot;
                    if (values == null)
                    {
                        stateText = "-";
                        dot = OffColor;
                    }
                    else
                    {
                        stateText = values[i] ? "ON" : "OFF";
                        dot = values[i] ? OnColor : OffColor;
                    }

                    int dotSize = Math.Min(12, rowHeight - 6);
                    int dotX = Width - 70;
                    using (var brush = new SolidBrush(dot))
                    {
                        g.FillEllipse(brush, dotX, y + (rowHeight - dotSize) / 2, dotSize, dotSize);
                    }
                    TextRenderer.DrawText(g, stateText, RowFont, new Rectangle(dotX + dotSize + 6, y, 44, rowHeight),
                        values != null && values[i] ? UiTheme.TextPrimary : UiTheme.TextSecondary,
                        TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
                }
            }
        }

        /// <summary>
        /// 축 하나의 상태 (CiA402 상태워드 해석 포함)
        /// </summary>
        internal sealed class ServoAxisSnapshot
        {
            public int Axis;
            public ushort StatusWord;
            public string Position;

            public static ServoAxisSnapshot Read(IEthercatDevice device, int axis)
            {
                return new ServoAxisSnapshot
                {
                    Axis = axis,
                    StatusWord = device.GetStatusWord(axis),
                    Position = axis == 1 ? device.Axis1_is_PosData() : device.Axis2_is_PosData()
                };
            }

            public bool Bit(int bit) => (StatusWord & (1 << bit)) != 0;

            /// <summary>
            /// CiA402 상태 이름 (상태워드 하위 비트 조합)
            /// </summary>
            public string StateName
            {
                get
                {
                    int masked = StatusWord & 0x6F;
                    if ((StatusWord & 0x4F) == 0x00) return "Not ready";
                    if ((StatusWord & 0x4F) == 0x40) return "Switch on disabled";
                    if (masked == 0x21) return "Ready to switch on";
                    if (masked == 0x23) return "Switched on";
                    if (masked == 0x27) return "Operation enabled (서보 ON)";
                    if (masked == 0x07) return "Quick stop active";
                    if ((StatusWord & 0x4F) == 0x0F) return "Fault reaction active";
                    if ((StatusWord & 0x4F) == 0x08) return "Fault (알람)";
                    return "알 수 없음";
                }
            }
        }

        /// <summary>
        /// 두 축의 상태워드, 상태, 주요 비트, 엔코더 위치를 표로 그립니다.
        /// </summary>
        private sealed class ServoStatusPanel : Control
        {
            private static readonly Font TitleFont = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            private static readonly Font RowFont = new Font("Segoe UI", 9F);

            private static readonly KeyValuePair<int, string>[] Bits =
            {
                new KeyValuePair<int, string>(2, "서보 ON"),
                new KeyValuePair<int, string>(3, "알람"),
                new KeyValuePair<int, string>(4, "전압"),
                new KeyValuePair<int, string>(7, "경고"),
                new KeyValuePair<int, string>(10, "목표 도달"),
                new KeyValuePair<int, string>(12, "bit12"),
                new KeyValuePair<int, string>(13, "bit13")
            };

            private ServoAxisSnapshot axis1;
            private ServoAxisSnapshot axis2;
            private string lastKey;

            public ServoStatusPanel()
            {
                SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
                    | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
                BackColor = UiTheme.Surface;
            }

            public void SetValues(ServoAxisSnapshot a1, ServoAxisSnapshot a2)
            {
                string key = a1 == null ? "-" : $"{a1.StatusWord}|{a1.Position}|{a2?.StatusWord}|{a2?.Position}";
                if (key == lastKey)
                {
                    return;
                }
                lastKey = key;
                axis1 = a1;
                axis2 = a2;
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.Clear(BackColor);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var pen = new Pen(UiTheme.Border))
                {
                    g.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
                }
                TextRenderer.DrawText(g, "서보 상태 (드라이브 상태워드)", TitleFont, new Rectangle(10, 6, Width - 20, 22), UiTheme.TextPrimary,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

                int rowHeight = 26;
                int y = 34;
                int colAxis = 10;
                int colWord = 120;
                int colState = 200;
                int colBits = 420;
                int colPos = Width - 150;

                Color head = UiTheme.TextSecondary;
                DrawCell(g, "축", colAxis, y, 100, rowHeight, head);
                DrawCell(g, "상태워드", colWord, y, 80, rowHeight, head);
                DrawCell(g, "상태", colState, y, 220, rowHeight, head);
                DrawCell(g, "주요 비트", colBits, y, colPos - colBits - 10, rowHeight, head);
                DrawCell(g, "엔코더 위치", colPos, y, 140, rowHeight, head);

                DrawAxisRow(g, "Axis1 (상하)", axis1, y + rowHeight, rowHeight, colAxis, colWord, colState, colBits, colPos);
                DrawAxisRow(g, "Axis2 (좌우)", axis2, y + rowHeight * 2, rowHeight, colAxis, colWord, colState, colBits, colPos);
            }

            private void DrawAxisRow(Graphics g, string name, ServoAxisSnapshot axis, int y, int rowHeight,
                int colAxis, int colWord, int colState, int colBits, int colPos)
            {
                DrawCell(g, name, colAxis, y, 100, rowHeight, UiTheme.TextPrimary);
                if (axis == null)
                {
                    DrawCell(g, "-", colWord, y, 80, rowHeight, UiTheme.TextSecondary);
                    return;
                }

                bool fault = axis.Bit(3);
                DrawCell(g, $"0x{axis.StatusWord:X4}", colWord, y, 80, rowHeight, UiTheme.TextPrimary);
                DrawCell(g, axis.StateName, colState, y, 220, rowHeight, fault ? UiTheme.Danger : UiTheme.TextPrimary);

                int x = colBits;
                foreach (var bit in Bits)
                {
                    bool on = axis.Bit(bit.Key);
                    Color dot = on ? (bit.Key == 3 ? UiTheme.Danger : bit.Key == 7 ? UiTheme.Warning : UiTheme.Success)
                        : Color.FromArgb(200, 206, 214);
                    int width = TextRenderer.MeasureText(bit.Value, RowFont).Width + 22;
                    if (x + width > colPos - 6)
                    {
                        break;
                    }
                    using (var brush = new SolidBrush(dot))
                    {
                        g.FillEllipse(brush, x, y + (rowHeight - 10) / 2, 10, 10);
                    }
                    DrawCell(g, bit.Value, x + 13, y, width - 13, rowHeight, on ? UiTheme.TextPrimary : UiTheme.TextSecondary);
                    x += width;
                }

                DrawCell(g, string.IsNullOrWhiteSpace(axis.Position) ? "-" : axis.Position, colPos, y, 140, rowHeight, UiTheme.TextPrimary);
            }

            private static void DrawCell(Graphics g, string text, int x, int y, int width, int height, Color color)
            {
                TextRenderer.DrawText(g, text, RowFont, new Rectangle(x, y, Math.Max(0, width), height), color,
                    TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            }
        }
    }
}
