using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SemiconductorUi.Models;
using SemiconductorUi.ViewModels;
using AppSettings = SemiconductorUi.AppSettings;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 웨이퍼 로딩/언로딩 관련 이벤트 핸들러
    /// </summary>
    public class WaferEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// WaferEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public WaferEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// FOUP 장착 토글 버튼 클릭 이벤트 핸들러
        /// FOUP A와 B의 장착 상태를 함께 토글합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonToggleFoupMount_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }

            // 기존 단일 토글은 숨김 처리 예정. 눌렸다면 A/B를 함께 토글하는 보조 동작만 수행.
            var newState = !(form.IsFoupAMounted && form.IsFoupBMounted);
            form.IsFoupAMounted = newState;
            form.IsFoupBMounted = newState;
            form.IsFoupMounted = form.IsFoupAMounted && form.IsFoupBMounted;
            form.AddLogMessage($"FOUP 장착 상태 일괄 변경: A={BoolToMountText(form.IsFoupAMounted)}, B={BoolToMountText(form.IsFoupBMounted)}", "INFO");
            form.UpdateFoupPreparationButtons();
            form.UpdateSimulationUi();
        }

        /// <summary>
        /// 웨이퍼 로딩 버튼 클릭 이벤트 핸들러
        /// FOUP A에 웨이퍼를 로딩합니다. 사용자에게 웨이퍼 개수를 입력받습니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonWaferLoading_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }
            if (!form.IsFoupAMounted)
            {
                MessageBox.Show("FOUP A가 미장착 상태입니다. 먼저 FOUP A를 장착해 주세요.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var count = form.PromptForWaferCount(form.UserWaferLoadCount, 1, AppSettings.MaxFoupCapacity);
            if (count == null)
            {
                return;
            }
            form.UserWaferLoadCount = count.Value;

            form.SetWaferLoadState(MainFormViewModel.WaferLoadStateType.Loading);
            form.AddLogMessage($"웨이퍼 로딩: {form.UserWaferLoadCount}장 설정", "INFO");
            
            // 웨이퍼 로딩 시 즉시 UI에 반영
            // FoupManager를 통해 웨이퍼 로딩 처리
            // FoupManager는 생성자에서 이미 초기화되므로 null 체크 불필요
            
            // 설정된 개수만큼 웨이퍼를 큐에 추가 (FoupManager를 통해 처리)
            form.foupManager?.LoadWafersToFoupA(form.UserWaferLoadCount, form.SecondExposureEnabled);
            
            // UI 업데이트
            form.UpdateSimulationUi();
        }

        /// <summary>
        /// 웨이퍼 언로딩 버튼 클릭 이벤트 핸들러
        /// FOUP B에서 완료된 웨이퍼를 언로딩합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonWaferUnloading_Click(object sender, EventArgs e)
        {
            if (!form.EnsureLoggedIn())
            {
                return;
            }
            if (!form.IsFoupBMounted)
            {
                MessageBox.Show("FOUP B가 미장착 상태입니다. 먼저 FOUP B를 장착해 주세요.", "준비 미완료", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // FOUP B 언로딩: 완료 웨이퍼 배출 처리
            form.UnloadFoupB();
            form.SetWaferLoadState(MainFormViewModel.WaferLoadStateType.Unloading);
        }

        /// <summary>
        /// 장착 상태를 텍스트로 변환
        /// </summary>
        /// <param name="mounted">장착 여부</param>
        /// <returns>"장착" 또는 "미장착"</returns>
        private string BoolToMountText(bool mounted)
        {
            return mounted ? "장착" : "미장착";
        }
    }
}

