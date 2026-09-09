using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using IEG3268_Dll;
using SemiconductorUi.Controllers;
using SemiconductorUi.Models;
using SemiconductorUi.Repositories;

namespace SemiconductorUi.Forms
{
    /// <summary>
    /// TM 티칭 위치(챔버/FOUP/원점·하강오프셋) 편집 폼
    /// </summary>
    public class TeachingPositionForm : Form
    {
        private readonly Func<IEG3268> _getEthercat;
        private readonly Func<bool> _isConnected;
        private readonly Action<TmHardwareController.TmPositionSet> _onApplied;

        private NumericUpDown numDescendOffset;
        private NumericUpDown numHomeX;
        private NumericUpDown numHomeY;

        private NumericUpDown numCaX, numCaLand, numCaRaise;
        private NumericUpDown numCbX, numCbLand, numCbRaise;
        private NumericUpDown numCcX, numCcLand, numCcRaise;
        private NumericUpDown numFaX, numFbX;

        private NumericUpDown[] numFaLand = new NumericUpDown[5];
        private NumericUpDown[] numFaRaise = new NumericUpDown[5];
        private NumericUpDown[] numFbLand = new NumericUpDown[5];
        private NumericUpDown[] numFbRaise = new NumericUpDown[5];

        private Label labelCurrentPos;
        private TeachingPositionsData _working;

        public TeachingPositionForm(
            TeachingPositionsData initial,
            Func<IEG3268> getEthercat = null,
            Func<bool> isConnected = null,
            Action<TmHardwareController.TmPositionSet> onApplied = null)
        {
            _getEthercat = getEthercat;
            _isConnected = isConnected;
            _onApplied = onApplied;
            _working = initial ?? TeachingPositionsRepository.Load();

            Text = "TM 티칭 위치 설정";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimizeBox = true;
            MaximizeBox = true;
            Size = new Size(920, 720);
            MinimumSize = new Size(860, 640);
            BackColor = Color.FromArgb(248, 249, 251);
            Font = new Font("Segoe UI", 9F);

            BuildUi();
            LoadToUi(_working);
            RefreshCurrentPositionLabel();
        }

        private void BuildUi()
        {
            var top = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                Padding = new Padding(12),
                BackColor = Color.FromArgb(236, 239, 244)
            };
            var title = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 28,
                Text = "챔버·FOUP 좌표와 Home(원점) 티칭값을 편집합니다. 저장 시 TeachingPositions.xml에 기록됩니다.",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold)
            };
            labelCurrentPos = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Fill,
                Text = "현재 서보 위치: (미연결)",
                TextAlign = ContentAlignment.MiddleLeft
            };
            top.Controls.Add(labelCurrentPos);
            top.Controls.Add(title);
            Controls.Add(top);

            var bottom = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(236, 239, 244)
            };
            var btnSave = MakeButton("저장 및 적용", Color.FromArgb(46, 125, 90));
            var btnCancel = MakeButton("닫기", Color.FromArgb(90, 90, 90));
            var btnReset = MakeButton("기본값 복원", Color.FromArgb(140, 70, 70));
            var btnReadPos = MakeButton("현재위치 읽기", Color.FromArgb(55, 95, 140));
            var btnRefresh = MakeButton("위치 새로고침", Color.FromArgb(80, 100, 120));
            btnSave.Click += (s, e) => SaveAndApply();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            btnReset.Click += (s, e) =>
            {
                if (MessageBox.Show(this, "코드에 내장된 기본 티칭값으로 되돌리겠습니까?\n(아직 저장되지 않습니다)", "기본값 복원",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
                LoadToUi(TeachingPositionsData.CreateDefault());
            };
            btnReadPos.Click += (s, e) => ReadCurrentIntoFocused();
            btnRefresh.Click += (s, e) => RefreshCurrentPositionLabel();
            bottom.Controls.Add(btnSave);
            bottom.Controls.Add(btnCancel);
            bottom.Controls.Add(btnReset);
            bottom.Controls.Add(btnReadPos);
            bottom.Controls.Add(btnRefresh);
            Controls.Add(bottom);

            var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(8, 4) };
            tabs.TabPages.Add(BuildCommonTab());
            tabs.TabPages.Add(BuildChamberTab("Chamber A", "A", out numCaX, out numCaLand, out numCaRaise));
            tabs.TabPages.Add(BuildChamberTab("Chamber B", "B", out numCbX, out numCbLand, out numCbRaise));
            tabs.TabPages.Add(BuildChamberTab("Chamber C", "C", out numCcX, out numCcLand, out numCcRaise));
            tabs.TabPages.Add(BuildFoupTab("FOUP A", true));
            tabs.TabPages.Add(BuildFoupTab("FOUP B", false));
            Controls.Add(tabs);
            tabs.BringToFront();
            top.SendToBack();
            bottom.SendToBack();
        }

        private TabPage BuildCommonTab()
        {
            var page = new TabPage("공통 / Home");
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(16)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            numDescendOffset = MakeLongNum();
            numHomeX = MakeLongNum();
            numHomeY = MakeLongNum();
            AddRow(panel, "하강 오프셋 (DescendOffset)", numDescendOffset);
            AddRow(panel, "Home X (좌우 / Axis2)", numHomeX);
            AddRow(panel, "Home Y (상하 / Axis1)", numHomeY);

            var hint = new Label
            {
                AutoSize = true,
                MaximumSize = new Size(800, 0),
                Margin = new Padding(16, 8, 16, 8),
                Text = "하강 위치 = 안착(Land) Y − 하강 오프셋.\n" +
                      "Home X/Y는 대기(TM) Region 좌표용입니다. 서보 '원점복귀' 버튼은 드라이브 HOME 센서 시퀀스이며 Home 좌표와는 별개입니다.\n" +
                      "현재위치 읽기: 포커스가 있는 숫자칸에 현재 서보 좌표를 넣습니다 (X칸←Axis2, Y칸←Axis1)."
            };
            var host = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            host.Controls.Add(hint);
            host.Controls.Add(panel);
            hint.Dock = DockStyle.Top;
            panel.Dock = DockStyle.Top;
            page.Controls.Add(host);
            return page;
        }

        private TabPage BuildChamberTab(string title, string key, out NumericUpDown x, out NumericUpDown land, out NumericUpDown raise)
        {
            var page = new TabPage(title);
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(16)
            };
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            x = MakeLongNum();
            land = MakeLongNum();
            raise = MakeLongNum();
            AddRow(panel, $"{key} X (좌우)", x);
            AddRow(panel, $"{key} 안착 Y (Land)", land);
            AddRow(panel, $"{key} 상승 Y (Raise)", raise);
            var descendHint = new Label
            {
                AutoSize = true,
                Margin = new Padding(16),
                Text = $"하강 Y는 저장값 안착Y − DescendOffset 으로 자동 계산됩니다."
            };
            var host = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            host.Controls.Add(descendHint);
            host.Controls.Add(panel);
            descendHint.Dock = DockStyle.Top;
            panel.Dock = DockStyle.Top;
            page.Controls.Add(host);
            return page;
        }

        private TabPage BuildFoupTab(string title, bool isA)
        {
            var page = new TabPage(title);
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 2,
                Padding = new Padding(16)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            var numX = MakeLongNum();
            if (isA) numFaX = numX; else numFbX = numX;
            AddRow(root, $"{title} X (좌우)", numX);

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 3,
                Padding = new Padding(16, 8, 16, 16)
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            grid.Controls.Add(new Label { Text = "층", AutoSize = true }, 0, 0);
            grid.Controls.Add(new Label { Text = "안착 Y (Land)", AutoSize = true }, 1, 0);
            grid.Controls.Add(new Label { Text = "상승 Y (Raise)", AutoSize = true }, 2, 0);

            for (int i = 0; i < 5; i++)
            {
                var land = MakeLongNum();
                var raise = MakeLongNum();
                if (isA)
                {
                    numFaLand[i] = land;
                    numFaRaise[i] = raise;
                }
                else
                {
                    numFbLand[i] = land;
                    numFbRaise[i] = raise;
                }
                grid.Controls.Add(new Label { Text = $"{i + 1}층", AutoSize = true, Anchor = AnchorStyles.Left }, 0, i + 1);
                grid.Controls.Add(land, 1, i + 1);
                grid.Controls.Add(raise, 2, i + 1);
            }

            var host = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            host.Controls.Add(grid);
            host.Controls.Add(root);
            grid.Dock = DockStyle.Top;
            root.Dock = DockStyle.Top;
            page.Controls.Add(host);
            return page;
        }

        private static void AddRow(TableLayoutPanel panel, string label, Control editor)
        {
            int row = panel.RowCount++;
            panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            panel.Controls.Add(new Label
            {
                Text = label,
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Margin = new Padding(3, 10, 3, 3)
            }, 0, row);
            panel.Controls.Add(editor, 1, row);
        }

        private static NumericUpDown MakeLongNum()
        {
            return new NumericUpDown
            {
                Minimum = -5000000,
                Maximum = 5000000,
                DecimalPlaces = 0,
                Increment = 1,
                Width = 180,
                ThousandsSeparator = false,
                Anchor = AnchorStyles.Left
            };
        }

        private static Button MakeButton(string text, Color back)
        {
            return new Button
            {
                Text = text,
                Width = 120,
                Height = 34,
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(6, 4, 6, 4)
            };
        }

        private void LoadToUi(TeachingPositionsData d)
        {
            _working = d ?? TeachingPositionsData.CreateDefault();
            SetNum(numDescendOffset, _working.DescendOffset);
            SetNum(numHomeX, _working.Home_X);
            SetNum(numHomeY, _working.Home_Y);
            SetNum(numCaX, _working.ChamberA_X);
            SetNum(numCaLand, _working.ChamberA_LandY);
            SetNum(numCaRaise, _working.ChamberA_RaiseY);
            SetNum(numCbX, _working.ChamberB_X);
            SetNum(numCbLand, _working.ChamberB_LandY);
            SetNum(numCbRaise, _working.ChamberB_RaiseY);
            SetNum(numCcX, _working.ChamberC_X);
            SetNum(numCcLand, _working.ChamberC_LandY);
            SetNum(numCcRaise, _working.ChamberC_RaiseY);
            SetNum(numFaX, _working.FoupA_X);
            SetNum(numFbX, _working.FoupB_X);
            for (int i = 0; i < 5; i++)
            {
                SetNum(numFaLand[i], SafeIndex(_working.FoupA_LandY, i));
                SetNum(numFaRaise[i], SafeIndex(_working.FoupA_RaiseY, i));
                SetNum(numFbLand[i], SafeIndex(_working.FoupB_LandY, i));
                SetNum(numFbRaise[i], SafeIndex(_working.FoupB_RaiseY, i));
            }
        }

        private TeachingPositionsData ReadFromUi()
        {
            var d = new TeachingPositionsData
            {
                DescendOffset = (long)numDescendOffset.Value,
                Home_X = (long)numHomeX.Value,
                Home_Y = (long)numHomeY.Value,
                ChamberA_X = (long)numCaX.Value,
                ChamberA_LandY = (long)numCaLand.Value,
                ChamberA_RaiseY = (long)numCaRaise.Value,
                ChamberB_X = (long)numCbX.Value,
                ChamberB_LandY = (long)numCbLand.Value,
                ChamberB_RaiseY = (long)numCbRaise.Value,
                ChamberC_X = (long)numCcX.Value,
                ChamberC_LandY = (long)numCcLand.Value,
                ChamberC_RaiseY = (long)numCcRaise.Value,
                FoupA_X = (long)numFaX.Value,
                FoupB_X = (long)numFbX.Value,
                FoupA_LandY = new long[5],
                FoupA_RaiseY = new long[5],
                FoupB_LandY = new long[5],
                FoupB_RaiseY = new long[5]
            };
            for (int i = 0; i < 5; i++)
            {
                d.FoupA_LandY[i] = (long)numFaLand[i].Value;
                d.FoupA_RaiseY[i] = (long)numFaRaise[i].Value;
                d.FoupB_LandY[i] = (long)numFbLand[i].Value;
                d.FoupB_RaiseY[i] = (long)numFbRaise[i].Value;
            }
            return d;
        }

        private void SaveAndApply()
        {
            try
            {
                var data = ReadFromUi();
                if (data.DescendOffset <= 0)
                {
                    MessageBox.Show(this, "하강 오프셋은 1 이상이어야 합니다.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TeachingPositionsRepository.Save(data);
                var set = new TmHardwareController.TmPositionSet();
                set.ApplyFrom(data);
                _onApplied?.Invoke(set);
                MessageBox.Show(this, "티칭 위치를 저장하고 적용했습니다.", "저장 완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"저장 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshCurrentPositionLabel()
        {
            try
            {
                if (_isConnected == null || !_isConnected() || _getEthercat == null || _getEthercat() == null)
                {
                    labelCurrentPos.Text = "현재 서보 위치: EtherCAT 미연결 (수동 입력만 가능)";
                    return;
                }

                var ec = _getEthercat();
                string xRaw = ec.Axis2_is_PosData();
                string yRaw = ec.Axis1_is_PosData();
                labelCurrentPos.Text = $"현재 서보 위치: X(Axis2)={xRaw}, Y(Axis1)={yRaw}";
            }
            catch (Exception ex)
            {
                labelCurrentPos.Text = $"현재 서보 위치 읽기 실패: {ex.Message}";
            }
        }

        private void ReadCurrentIntoFocused()
        {
            RefreshCurrentPositionLabel();
            if (_isConnected == null || !_isConnected() || _getEthercat == null || _getEthercat() == null)
            {
                MessageBox.Show(this, "EtherCAT이 연결되어야 현재 위치를 읽을 수 있습니다.", "연결 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var ec = _getEthercat();
                if (!TryParseLong(ec.Axis2_is_PosData(), out long x) || !TryParseLong(ec.Axis1_is_PosData(), out long y))
                {
                    MessageBox.Show(this, "서보 위치 문자열을 숫자로 변환하지 못했습니다.", "읽기 실패", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var focused = FindFocusedNumeric(this);
                if (focused == null)
                {
                    MessageBox.Show(this, "값을 넣을 Numeric 칸을 먼저 클릭하세요.\n(이름에 X가 있으면 Axis2, Y면 Axis1)", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // X 칸 추정: Home X / Chamber X / FOUP X
                bool likelyX = focused == numHomeX || focused == numCaX || focused == numCbX || focused == numCcX || focused == numFaX || focused == numFbX;
                bool likelyY = focused == numHomeY || focused == numCaLand || focused == numCaRaise
                    || focused == numCbLand || focused == numCbRaise || focused == numCcLand || focused == numCcRaise
                    || Array.IndexOf(numFaLand, focused) >= 0 || Array.IndexOf(numFaRaise, focused) >= 0
                    || Array.IndexOf(numFbLand, focused) >= 0 || Array.IndexOf(numFbRaise, focused) >= 0
                    || focused == numDescendOffset;

                if (likelyX)
                {
                    SetNum(focused, x);
                }
                else if (likelyY || !likelyX)
                {
                    // 기본: Y(상하) 칸으로 간주
                    SetNum(focused, likelyY ? y : y);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, $"현재 위치 반영 오류: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static NumericUpDown FindFocusedNumeric(Control root)
        {
            if (root is NumericUpDown n && n.Focused) return n;
            foreach (Control c in root.Controls)
            {
                var found = FindFocusedNumeric(c);
                if (found != null) return found;
            }
            return null;
        }

        private static void SetNum(NumericUpDown num, long value)
        {
            if (num == null) return;
            if (value < (long)num.Minimum) value = (long)num.Minimum;
            if (value > (long)num.Maximum) value = (long)num.Maximum;
            num.Value = value;
        }

        private static long SafeIndex(long[] arr, int i)
        {
            if (arr == null || i < 0 || i >= arr.Length) return 0;
            return arr[i];
        }

        private static bool TryParseLong(string raw, out long value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(raw) || raw == "-") return false;
            return long.TryParse(raw.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value)
                || long.TryParse(raw.Trim(), NumberStyles.Integer, CultureInfo.CurrentCulture, out value);
        }
    }
}
