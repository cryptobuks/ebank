using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Calendars;
using Infrastructure;

namespace Application.CalendarProcessing
{
    public class CalendarService
    {
        private IUnitOfWork _unitOfWork;

        public Calendar Calendar
        {
            get { return _unitOfWork.CalendarRepository.GetAll().First(); }
        }

        public CalendarService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // TODO: try TimeSpan if it works well for all time cases
        // TODO: all DateTime.UtcNow should be replaced with exceptions
        public DateTime MoveTime(byte days)
        {
            var currentCalendar = _unitOfWork.CalendarRepository.GetAll().First();
            var result = currentCalendar.CurrentTime.HasValue ? currentCalendar.CurrentTime.Value : DateTime.UtcNow;
            if (!currentCalendar.ProcessingLock)
            {
                currentCalendar.ProcessingLock = true;
                _unitOfWork.CalendarRepository.Upsert(currentCalendar);
                _unitOfWork.Save();
                var calendar2 = _unitOfWork.CalendarRepository.GetAll().First();
                if (calendar2.Equals(currentCalendar))
                {
                    calendar2.CurrentTime = calendar2.CurrentTime.HasValue
                        ? calendar2.CurrentTime.Value.AddDays(days)
                        : DateTime.UtcNow;
                    result = calendar2.CurrentTime.Value;
                    calendar2.ProcessingLock = false;
                    // TODO: is it needed to update it explicitly?
                    _unitOfWork.CalendarRepository.Upsert(calendar2);
                    _unitOfWork.Save();
                }
                else throw new Exception("Calendar is locked");
            }
            return result;
        }

        public void UpdateDailyProcessingTime()
        {
            var currentCalendar = _unitOfWork.CalendarRepository.GetAll().First();
            currentCalendar.LastDailyProcessingTime = currentCalendar.CurrentTime;
            _unitOfWork.CalendarRepository.Upsert(currentCalendar); // TODO: can it be removed?
            _unitOfWork.Save();
        }

        public void UpdateMonthlyProcessingTime()
        {
            var currentCalendar = _unitOfWork.CalendarRepository.GetAll().First();
            currentCalendar.LastMonthlyProcessingTime = currentCalendar.CurrentTime;
            _unitOfWork.CalendarRepository.Upsert(currentCalendar); // TODO: can it be removed?
            _unitOfWork.Save();
        }

        internal void SetCurrentDate(DateTime dateTime)
        {
            Calendar calendar = null;
            if (_unitOfWork.CalendarRepository.GetAll().Any())
            {
                _unitOfWork.CalendarRepository.GetAll().First().CurrentTime = dateTime;
            }
            else
            {
                calendar = new Calendar { Id = Calendar.ConstGuid, CurrentTime = dateTime };
            }
            if (calendar != null)
            {
                _unitOfWork.CalendarRepository.Upsert(calendar);
            }
            else
            {
                throw new Exception("Something went wrong on setting current date");
            }
        }
    }
}
