using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Models;
using SemiconductorUi.Repositories;

namespace SemiconductorUi.Forms
{
	public partial class RecipeManagerForm : Form
	{
		private List<RecipeSnapshot> _recipes = new List<RecipeSnapshot>();
		public Action<List<RecipeSnapshot>> OnSavedAll;

		public RecipeManagerForm()
		{
			InitializeComponent();
			try
			{
				// SplitContainer 안전 초기화 (초기 폭이 0일 수 있어 최소 제약 해제 후 설정)
				if (splitContainer1 != null)
				{
					splitContainer1.Panel1MinSize = 0;
					splitContainer1.Panel2MinSize = 0;
					splitContainer1.SplitterWidth = 4;
					ApplySplitterRatio();
				}

				LoadData();
			}
			catch
			{
				_recipes = RecipeRepository.EnsureSeedDefaults();
			}
			try
			{
				BindList();
			}
			catch
			{
				// 리스트 바인딩 실패 시에도 폼은 열리도록 방어
			}
			// 폼이 실제로 레이아웃된 후에도 한 번 더 안전 보정
			this.Load += (s, e) =>
			{
				try
				{
					ApplySplitterRatio();
				}
				catch { }
			};
			// 리사이즈 시에도 비율 유지
			this.SizeChanged += (s, e) =>
			{
				try { ApplySplitterRatio(); } catch { }
			};
		}

		private void ApplySplitterRatio()
		{
			if (splitContainer1 == null) return;
			int total = Math.Max(1, splitContainer1.Width > 0 ? splitContainer1.Width : this.ClientSize.Width);
			// 좌측 목록 영역을 전체의 32%로 유지, 최소 220, 최대 420
			int desired = (int)Math.Round(total * 0.32);
			int panel1 = Math.Max(220, Math.Min(420, desired));
			// Panel2가 200 이상 남도록 보장
			int maxAllowed = Math.Max(0, total - 200);
			splitContainer1.SplitterDistance = Math.Min(panel1, maxAllowed);
		}

		private void LoadData()
		{
			_recipes = RecipeRepository.LoadAll();
		}

		private void BindList()
		{
			listRecipes.BeginUpdate();
			listRecipes.Items.Clear();
			foreach (var r in _recipes.OrderBy(x => x.Name ?? string.Empty))
			{
				listRecipes.Items.Add(r.Name ?? "(이름없음)");
			}
			listRecipes.EndUpdate();
		}

		private void listRecipes_SelectedIndexChanged(object sender, EventArgs e)
		{
			try
			{
				var idx = listRecipes.SelectedIndex;
				if (idx < 0 || idx >= _recipes.Count) return;
				var r = _recipes.OrderBy(x => x.Name ?? string.Empty).ElementAt(idx);
				FillEditor(r);
			}
			catch
			{
				// 무시: 부분 선택 상태 등에서 안정적으로 넘어가기 위함
			}
		}

		private void FillEditor(RecipeSnapshot r)
		{
			txtName.Text = r.Name ?? string.Empty;
			numWafer.Value = Clamp(r.WaferCount, (int)numWafer.Minimum, (int)numWafer.Maximum);
			numDurA.Value = Clamp(r.DurA, (int)numDurA.Minimum, (int)numDurA.Maximum);
			numDurB.Value = Clamp(r.DurB, (int)numDurB.Minimum, (int)numDurB.Maximum);
			numDurC.Value = Clamp(r.DurC, (int)numDurC.Minimum, (int)numDurC.Maximum);
			chkSecond.Checked = r.SecondExposure;

			SetNum(pmaT, r.PMA?.T ?? 0); SetNum(pmaP, r.PMA?.P ?? 0); SetNum(pmaH, r.PMA?.H ?? 0);
			SetNum(pmbT, r.PMB?.T ?? 0); SetNum(pmbP, r.PMB?.P ?? 0); SetNum(pmbH, r.PMB?.H ?? 0);
			SetNum(pmcT, r.PMC?.T ?? 0); SetNum(pmcP, r.PMC?.P ?? 0); SetNum(pmcH, r.PMC?.H ?? 0);

			SetNum(gasNF3, r.GasRfPMA?.NF3 ?? 0);
			SetNum(gasO2, r.GasRfPMA?.O2 ?? 0);
			SetNum(gasCF4, r.GasRfPMA?.CF4 ?? 0);
			SetNum(gasRF, r.GasRfPMA?.RF ?? 0);
			// PMB/PMC 가스RF (존재하지 않을 수 있어 0으로 대체)
			var b = r.GasRfPMB ?? new RecipeSnapshot.GasRf();
			var c = r.GasRfPMC ?? new RecipeSnapshot.GasRf();
			var ctlB = FindGasControlsFor("PMB");
			var ctlC = FindGasControlsFor("PMC");
			if (ctlB != null) { SetNum(ctlB.Item1, b.NF3); SetNum(ctlB.Item2, b.O2); SetNum(ctlB.Item3, b.CF4); SetNum(ctlB.Item4, b.RF); }
			if (ctlC != null) { SetNum(ctlC.Item1, c.NF3); SetNum(ctlC.Item2, c.O2); SetNum(ctlC.Item3, c.CF4); SetNum(ctlC.Item4, c.RF); }
		}

		private void ReadEditorInto(RecipeSnapshot r)
		{
			r.Name = txtName.Text?.Trim();
			r.WaferCount = (int)numWafer.Value;
			r.DurA = (int)numDurA.Value;
			r.DurB = (int)numDurB.Value;
			r.DurC = (int)numDurC.Value;
			r.SecondExposure = chkSecond.Checked;
			r.PMA = r.PMA ?? new RecipeSnapshot.Triple(); r.PMA.T = (double)pmaT.Value; r.PMA.P = (double)pmaP.Value; r.PMA.H = (double)pmaH.Value;
			r.PMB = r.PMB ?? new RecipeSnapshot.Triple(); r.PMB.T = (double)pmbT.Value; r.PMB.P = (double)pmbP.Value; r.PMB.H = (double)pmbH.Value;
			r.PMC = r.PMC ?? new RecipeSnapshot.Triple(); r.PMC.T = (double)pmcT.Value; r.PMC.P = (double)pmcP.Value; r.PMC.H = (double)pmcH.Value;
			r.GasRfPMA = r.GasRfPMA ?? new RecipeSnapshot.GasRf(); r.GasRfPMA.NF3 = (double)gasNF3.Value; r.GasRfPMA.O2 = (double)gasO2.Value; r.GasRfPMA.CF4 = (double)gasCF4.Value; r.GasRfPMA.RF = (double)gasRF.Value;
			// PMB/PMC
			var ctlB = FindGasControlsFor("PMB");
			if (ctlB != null)
			{
				r.GasRfPMB = r.GasRfPMB ?? new RecipeSnapshot.GasRf();
				r.GasRfPMB.NF3 = (double)ctlB.Item1.Value;
				r.GasRfPMB.O2  = (double)ctlB.Item2.Value;
				r.GasRfPMB.CF4 = (double)ctlB.Item3.Value;
				r.GasRfPMB.RF  = (double)ctlB.Item4.Value;
			}
			var ctlC = FindGasControlsFor("PMC");
			if (ctlC != null)
			{
				r.GasRfPMC = r.GasRfPMC ?? new RecipeSnapshot.GasRf();
				r.GasRfPMC.NF3 = (double)ctlC.Item1.Value;
				r.GasRfPMC.O2  = (double)ctlC.Item2.Value;
				r.GasRfPMC.CF4 = (double)ctlC.Item3.Value;
				r.GasRfPMC.RF  = (double)ctlC.Item4.Value;
			}
		}

		private void btnNew_Click(object sender, EventArgs e)
		{
			var r = new RecipeSnapshot { Name = "새 레시피" };
			_recipes.Add(r);
			BindList();
			SelectByName(r.Name);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			var idx = listRecipes.SelectedIndex;
			RecipeSnapshot target;
			if (idx >= 0 && idx < _recipes.Count)
			{
				target = _recipes.OrderBy(x => x.Name ?? string.Empty).ElementAt(idx);
			}
			else
			{
				// 선택되지 않은 상태에서는 새로 생성하지 않고 막는다
				MessageBox.Show(this, "수정할 레시피를 먼저 선택하세요.\r\n신규 레시피를 만들려면 '신규' 버튼을 사용하세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			ReadEditorInto(target);
			// 필수값 검증: 이름 비어있으면 저장 불가
			if (string.IsNullOrWhiteSpace(target.Name))
			{
				MessageBox.Show(this, "레시피 이름을 입력하세요.", "안내", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return;
			}
			
				RecipeRepository.SaveAll(_recipes);
					BindList();
					SelectByName(target.Name);
					OnSavedAll?.Invoke(_recipes);
					MessageBox.Show(this, "저장/수정이 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			var idx = listRecipes.SelectedIndex;
			if (idx < 0) return;
			var r = _recipes.Ordered()[idx];
			if (MessageBox.Show(this, $"'{r.Name}' 레시피를 삭제하시겠습니까?", "확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
					_recipes.Remove(r);
					RecipeRepository.SaveAll(_recipes);
						BindList();
						ClearEditor();
						OnSavedAll?.Invoke(_recipes);
			}
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void ClearEditor()
		{
			txtName.Text = string.Empty;
			numWafer.Value = numWafer.Minimum;
			numDurA.Value = numDurA.Minimum;
			numDurB.Value = numDurB.Minimum;
			numDurC.Value = numDurC.Minimum;
			chkSecond.Checked = false;
			foreach (var n in new[] { pmaT, pmaP, pmaH, pmbT, pmbP, pmbH, pmcT, pmcP, pmcH, gasNF3, gasO2, gasCF4, gasRF })
			{
				if (n != null) n.Value = n.Minimum;
			}
		}

		private void SetNum(NumericUpDown n, double v)
		{
			if (n == null) return;
			var dec = (decimal)v;
			if (dec < n.Minimum) dec = n.Minimum;
			if (dec > n.Maximum) dec = n.Maximum;
			n.Value = dec;
		}

		private void SelectByName(string name)
		{
			if (string.IsNullOrEmpty(name)) return;
			var items = listRecipes.Items.Cast<object>().Select(o => o?.ToString() ?? string.Empty).ToList();
			var idx = items.FindIndex(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase));
			if (idx >= 0) listRecipes.SelectedIndex = idx;
		}

		private int Clamp(int v, int min, int max) => v < min ? min : (v > max ? max : v);

		// 유틸: 레이아웃에서 PMB/PMC 가스 NumericUpDown 찾기
		private Tuple<NumericUpDown, NumericUpDown, NumericUpDown, NumericUpDown> FindGasControlsFor(string unit)
		{
			string prefix = unit == "PMB" ? "gasB_" : "gasC_";
			var nf3 = this.Controls.Find(prefix + "NF3", true).FirstOrDefault() as NumericUpDown;
			var o2  = this.Controls.Find(prefix + "O2",  true).FirstOrDefault() as NumericUpDown;
			var cf4 = this.Controls.Find(prefix + "CF4", true).FirstOrDefault() as NumericUpDown;
			var rf  = this.Controls.Find(prefix + "RF",  true).FirstOrDefault() as NumericUpDown;
			if (nf3 == null || o2 == null || cf4 == null || rf == null) return null;
			return Tuple.Create(nf3, o2, cf4, rf);
		}
	}

	internal static class RecipeListExtensions
	{
		public static List<RecipeSnapshot> Ordered(this List<RecipeSnapshot> src)
		{
			return src.OrderBy(x => x.Name ?? string.Empty).ToList();
		}
	}
}

