using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();

        CreateMap<EventDto, Event>()
            .ForMember(dest => dest.OrganizerId, opt => opt.MapFrom(src => src.IdOrganizer))
            .ForMember(dest => dest.IdStatus, opt => opt.MapFrom(src => src.IdStatus))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time))
            .ReverseMap()
            .ForMember(dest => dest.IdOrganizer, opt => opt.MapFrom(src => src.OrganizerId))
            .ForMember(dest => dest.IdStatus, opt => opt.MapFrom(src => src.IdStatus))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
            .ForMember(dest => dest.Time, opt => opt.MapFrom(src => src.Time));

        CreateMap<TaskDto, Task>()
            .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.IdEvent))
            .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.IdEmployee))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src =>
                DateTime.SpecifyKind(src.Date.ToDateTime(TimeOnly.MinValue), DateTimeKind.Utc)))
            .ReverseMap()
            .ForMember(dest => dest.IdEvent, opt => opt.MapFrom(src => src.EventId))
            .ForMember(dest => dest.IdEmployee, opt => opt.MapFrom(src => src.EmployeeId))
            .ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));

        // DateOnly <-> DateTime
        CreateMap<DateOnly, DateTime>().ConvertUsing(d => d.ToDateTime(TimeOnly.MinValue));
        CreateMap<DateTime, DateOnly>().ConvertUsing(d => DateOnly.FromDateTime(d));

        // TimeOnly <-> TimeSpan
        CreateMap<TimeOnly, TimeSpan>().ConvertUsing(t => t.ToTimeSpan());
        CreateMap<TimeSpan, TimeOnly>().ConvertUsing(t => TimeOnly.FromTimeSpan(t));

        CreateMap<EventGuest, EventGuestDto>().ReverseMap();
    }
}