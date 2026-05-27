using System;
using System.Drawing;
using SemiconductorUi.Controllers;
using SemiconductorUi.Controls;
using SemiconductorUi.Models;
using SemiconductorUi.ViewModels;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 웨이퍼 트래킹 및 상태 관리 서비스
    /// 웨이퍼 위치 추적, 2차 노광 관리, 웨이퍼 색상 결정 등
    /// </summary>
    public class WaferTrackingService
    {
        private readonly Form1 _form;

        /// <summary>
        /// WaferTrackingService 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public WaferTrackingService(Form1 form)
        {
            _form = form ?? throw new ArgumentNullException(nameof(form));
        }

        /// <summary>
        /// 웨이퍼가 챔버로 이동 중인지 확인
        /// </summary>
        /// <param name="chamber">챔버 상태</param>
        /// <returns>이동 중이면 true</returns>
        public bool IsWaferEnRouteToChamber(ChamberController.ChamberState chamber)
        {
            if (chamber == null || _form.CurrentTransfer?.DestinationChamber != chamber)
            {
                return false;
            }

            return _form.TmPhase == TransferController.TmPhase.DropoffExtend
                   || _form.TmPhase == TransferController.TmPhase.DropoffRetract
                   || _form.TmPhase == TransferController.TmPhase.WaitDoorDropoffClose;
        }

        /// <summary>
        /// 웨이퍼가 픽업 대기 중인지 확인
        /// </summary>
        /// <param name="chamber">챔버 상태</param>
        /// <returns>픽업 대기 중이면 true</returns>
        public bool IsWaferAwaitingPickup(ChamberController.ChamberState chamber)
        {
            if (chamber == null || _form.CurrentTransfer?.SourceChamber != chamber)
            {
                return false;
            }

            return _form.TmPhase == TransferController.TmPhase.MoveToPickup
                   || _form.TmPhase == TransferController.TmPhase.WaitDoorPickupOpen;
        }

        /// <summary>
        /// 웨이퍼가 2차 노광이 필요한지 확인
        /// </summary>
        /// <param name="wafer">웨이퍼</param>
        /// <returns>2차 노광이 필요하면 true</returns>
        public bool NeedsSecondExposure(Wafer wafer)
        {
            return _form.SecondExposureEnabled && wafer != null && wafer.RequiresSecondExposure && !wafer.SecondExposureCompleted;
        }

        /// <summary>
        /// 웨이퍼의 2차 노광 완료 표시
        /// </summary>
        /// <param name="wafer">웨이퍼</param>
        public void MarkSecondExposureComplete(Wafer wafer)
        {
            if (wafer == null)
            {
                return;
            }

            wafer.SecondExposureCompleted = true;
        }

        /// <summary>
        /// 챔버 상태에 따른 웨이퍼 색상 결정
        /// </summary>
        /// <param name="chamber">챔버 상태</param>
        /// <returns>웨이퍼 색상</returns>
        public Color GetWaferColorForState(ChamberController.ChamberState chamber)
        {
            if (chamber?.CurrentWafer == null)
            {
                return Color.Transparent;
            }

            var progress = chamber.TotalSeconds <= 0
                ? 0f
                : 1f - Math.Max(0, Math.Min(1, (float)chamber.RemainingSeconds / chamber.TotalSeconds));

            return progress < 0.5f
                ? AppSettings.WaferColorEarlyProcess
                : AppSettings.WaferColorLateProcess;
        }

        /// <summary>
        /// Region에 따른 웨이퍼 색상 결정
        /// </summary>
        /// <param name="region">장비 영역</param>
        /// <returns>웨이퍼 색상</returns>
        public Color GetWaferColorForRegion(EquipmentRegion region)
        {
            ChamberController.ChamberState chamber = null;
            switch (region)
            {
                case EquipmentRegion.ChamberA:
                    chamber = _form.ChamberAState;
                    break;
                case EquipmentRegion.ChamberB:
                    chamber = _form.ChamberBState;
                    break;
                case EquipmentRegion.ChamberC:
                    chamber = _form.ChamberCState;
                    break;
            }

            if (chamber == null)
            {
                return Color.FromArgb(210, 230, 255);
            }

            var color = GetWaferColorForState(chamber);
            return color == Color.Transparent ? Color.FromArgb(210, 230, 255) : color;
        }

        /// <summary>
        /// 웨이퍼가 블레이드에 있는지 확인 (실제 위치 기반)
        /// FOUP/Chamber에서 웨이퍼가 사라지는 순간(픽업 시작)부터 목적지에 안착하기 전까지
        /// </summary>
        /// <returns>블레이드에 웨이퍼가 있으면 true</returns>
        public bool IsWaferOnBlade()
        {
            if (_form.CurrentTransfer == null || _form.CurrentTransfer.Wafer == null)
            {
                return false;
            }

            // 웨이퍼가 픽업 시작 시점부터 드롭오프 완료 전까지 블레이드에 있음
            switch (_form.TmPhase)
            {
                // 픽업 단계 (웨이퍼가 FOUP/Chamber에서 사라지는 시점)
                case TransferController.TmPhase.WaitDoorPickupOpen:
                case TransferController.TmPhase.PickupExtend_ServoDown:  // 서보 하강 후 웨이퍼 접촉 (하드웨어 모드)
                case TransferController.TmPhase.PickupExtend_VacuumOn:   // 진공 ON 후 웨이퍼 확보 (하드웨어 모드)
                
                // 픽업 후 복귀 및 이동
                case TransferController.TmPhase.PickupRetract:
                case TransferController.TmPhase.PickupRetract_ServoUp:
                case TransferController.TmPhase.PickupRetract_CylinderBackward:
                case TransferController.TmPhase.WaitDoorPickupClose:
                case TransferController.TmPhase.MoveToDropoff:
                case TransferController.TmPhase.MoveToDropoff_WaitHardware:
                
                // 드롭오프 단계 (목적지에 도착했지만 아직 안착 전)
                case TransferController.TmPhase.WaitDoorDropoffOpen:
                case TransferController.TmPhase.DropoffExtend:
                case TransferController.TmPhase.DropoffExtend_CylinderForward:
                case TransferController.TmPhase.DropoffExtend_ServoDown:
                    return true;
                
                // 드롭오프 완료 후에는 블레이드에 없음
                default:
                    return false;
            }
        }

        /// <summary>
        /// Chamber 공정 시작 처리
        /// 중요: 실제 장비가 돌아가는 로직이므로 정확한 상태 관리 필수
        /// </summary>
        /// <param name="chamber">챔버 상태</param>
        /// <param name="wafer">웨이퍼</param>
        public void StartChamberProcessing(ChamberController.ChamberState chamber, Wafer wafer)
        {
            if (chamber == null || wafer == null)
            {
                _form.AddLogMessage($"공정 시작 실패: Chamber 또는 Wafer가 null입니다", "ERROR");
                return;
            }

            // 중복 호출 방지: 이미 공정이 시작된 상태면 스킵
            if (chamber.StatusText == "처리 중" || chamber.StatusText == "2차 노광 중")
            {
                // 이미 공정 중이면 중복 호출 방지
                return;
            }

            // 공정 상태 초기화
            chamber.ProcessingAccumulator = 0;
            chamber.LastProcessedTime = DateTime.MinValue; // 하드웨어 모드에서 첫 호출 시 설정됨
            chamber.ReservedForIncoming = false;
            wafer.CurrentStage = chamber.UnitKey;

            // 공정 상태 설정 (2차 노광 모드 고려)
            if (_form.SecondExposureEnabled && chamber == _form.ChamberCState)
            {
                chamber.StatusText = "2차 노광 중";
            }
            else if (chamber == _form.ChamberBState && NeedsSecondExposure(wafer))
            {
                chamber.StatusText = "2차 노광 중";
            }
            else
            {
                chamber.StatusText = "처리 중";
            }

            _form.AddLogMessage($"{chamber.UnitKey} 공정 시작: 웨이퍼 #{wafer.Id} (예상 시간: {chamber.TotalSeconds}초)", "INFO");
        }
    }
}

