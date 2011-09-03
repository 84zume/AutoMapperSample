using System;
using AutoMapper;

namespace AutoMapperSample
{
    class Program
    {
        static void Main()
        {
            var srcEmp = new SrcEmployee
                             {
                                 Name = "Sample Brother",
                                 Age = "24",
                                 Status = "退職",
                                 HireDate = Convert.ToDateTime("2010/2/1")
                             };

            //Mapper全体で使用する変換ルール
            Mapper.CreateMap<string, int>().ConvertUsing(Convert.ToInt32);
            Mapper.CreateMap<DateTime, string>().ConvertUsing<CustomDateTimeConverter>();

            //SrcEmployee -> DestEmployeeの変換のみで使用するルール
            Mapper.CreateMap<SrcEmployee, DestEmployee>()
                .ForMember(dest => dest.Status, opt => opt.ResolveUsing<EmployeeStatusResolver>());

            var destEmp = Mapper.Map<SrcEmployee, DestEmployee>(srcEmp);

            Console.WriteLine("Name:{0},Age:{1},Status:{2},HireDate:{3}", destEmp.Name, destEmp.Age, destEmp.Status, destEmp.HireDate);
            Console.ReadKey();

        }
    }


    /// <summary>
    /// 変換元のクラス
    /// </summary>
    public class SrcEmployee
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Status { get; set; }
        public DateTime HireDate { get; set; }
    }

    /// <summary>
    /// 変換先のクラス
    /// </summary>
    public class DestEmployee
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public EmployeeStatus Status { get; set; }
        public string HireDate { get; set; }
    }

    public enum EmployeeStatus { Stay, Exit, LongHoliday } ;


    /// <summary>
    /// 独自の DateTime -> string 型変換
    /// </summary>
    public class CustomDateTimeConverter : TypeConverter<DateTime, string>
    {
        protected override string ConvertCore(DateTime source)
        {
            return source.ToString("yyyy年MM月dd日");
        }
    }

    /// <summary>
    /// 独自の値変換
    /// </summary>
    public class EmployeeStatusResolver : ValueResolver<SrcEmployee, EmployeeStatus>
    {
        protected override EmployeeStatus ResolveCore(SrcEmployee source)
        {
            switch (source.Status)
            {
                case "在職":
                    return EmployeeStatus.Stay;
                case "退職":
                    return EmployeeStatus.Exit;
                case "長期休暇中":
                    return EmployeeStatus.LongHoliday;
                default:
                    throw new UnKnowonEmployeeStatusException();
            }
        }
    }

    public class UnKnowonEmployeeStatusException : Exception { }
}
