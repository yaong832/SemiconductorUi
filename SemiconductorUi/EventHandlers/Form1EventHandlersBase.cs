using System;
using System.Windows.Forms;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// Form1 이벤트 핸들러의 기본 클래스
    /// 모든 이벤트 핸들러 클래스가 Form1 인스턴스에 접근할 수 있도록 함
    /// </summary>
    public abstract class Form1EventHandlersBase
    {
        protected readonly Form1 form;

        protected Form1EventHandlersBase(Form1 form)
        {
            this.form = form ?? throw new ArgumentNullException(nameof(form));
        }
    }
}

