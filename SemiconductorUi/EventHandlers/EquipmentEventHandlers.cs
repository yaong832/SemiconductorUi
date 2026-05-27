using System;
using System.Windows.Forms;
using SemiconductorUi.Forms;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// 장비 제어 및 클릭 이벤트 핸들러
    /// </summary>
    public class EquipmentEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// EquipmentEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public EquipmentEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// 장비 패널 클릭 이벤트 핸들러
        /// 클릭된 장비의 상세 정보를 표시합니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void EquipmentPanel_Click(object sender, EventArgs e)
        {
            var control = sender as Control;
            var unitKey = control?.Tag as string;
            if (string.IsNullOrEmpty(unitKey))
            {
                return;
            }

            form.HandleEquipmentUnitClick(unitKey);
        }

        /// <summary>
        /// Chamber A 환경 상세 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonPMAEnvDetail_Click(object sender, EventArgs e)
        {
            form.ShowPmEnvironmentDetail("PMA");
        }

        /// <summary>
        /// Chamber B 환경 상세 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonPMBEnvDetail_Click(object sender, EventArgs e)
        {
            form.ShowPmEnvironmentDetail("PMB");
        }

        /// <summary>
        /// Chamber C 환경 상세 버튼 클릭 이벤트 핸들러
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">이벤트 인자</param>
        public void ButtonPMCEnvDetail_Click(object sender, EventArgs e)
        {
            form.ShowPmEnvironmentDetail("PMC");
        }
    }
}

