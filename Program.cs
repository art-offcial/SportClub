/*
    Проект: Справочно-информационная система "Спортивный клуб"
    Язык: C#
    Выполнил: студент Art Jud
    Группа: 9/2-РПО-24/1

    Исправленная версия
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace SportClub
{
    // для себя: использовал Serializable, потому что бинарная сериализация проще всего
    // сначала пытался через BinaryWriter вручную, но запутался в полях
    // Serializable позволяет одной строчкой сохранить весь объект
    [Serializable]
    public class Member
    {
        // поля сделал публичными, потому что класс простой и не требует сложной инкапсуляции
        public int Id;          // уникальный идентификатор клиента
        public string LastName; // фамилия
        public string FirstName; // имя
        public string Phone;     // номер телефона (строка, чтобы можно было с плюсом и скобками)
        public string RegDate;   // дата регистрации, храню как строку, чтобы не возиться с DateTime

        // конструктор для удобного создания объекта
        public Member(int id, string lastName, string firstName, string phone, string regDate)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            Phone = phone;
            RegDate = regDate;
        }
    }

    // структура для тренера — аналогично клиенту
    [Serializable]
    public class Trainer
    {
        public int Id;                 // ID тренера
        public string LastName;        // фамилия
        public string FirstName;       // имя
        public string Specialization;  // чем занимается (йога, бокс, плавание и т.д.)
        public int Experience;         // стаж в годах, целое число

        public Trainer(int id, string lastName, string firstName, string specialization, int experience)
        {
            Id = id;
            LastName = lastName;
            FirstName = firstName;
            Specialization = specialization;
            Experience = experience;
        }
    }

    // структура для тренировки — связывает клиента и тренера
    [Serializable]
    public class Workout
    {
        public int Id;           // ID тренировки
        public int MemberId;     // ссылка на клиента (внешний ключ)
        public int TrainerId;    // ссылка на тренера (внешний ключ)
        public string WorkoutDate; // дата проведения
        public int Duration;     // длительность в минутах

        public Workout(int id, int memberId, int trainerId, string workoutDate, int duration)
        {
            Id = id;
            MemberId = memberId;
            TrainerId = trainerId;
            WorkoutDate = workoutDate;
            Duration = duration;
        }
    }

    class Program
    {
        // пути к файлам: решил использовать BaseDirectory, чтобы не было проблем с переносом на другой компьютер
        // AppDomain.CurrentDomain.BaseDirectory — это папка, где лежит .exe файл
        // так файлы всегда будут рядом с программой, независимо от того, куда её скопировали
        static string memberFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "members.bin");
        static string trainerFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "trainers.bin");
        static string workoutFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workouts.bin");

        // сделал обобщённый метод, чтобы не писать три одинаковых метода для сохранения
        // T — это тип: Member, Trainer или Workout. Один метод работает для всех
        // FileMode.Create — перезаписывает файл, если он существует
        static void SaveList<T>(List<T> list, string filePath)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(filePath, FileMode.Create))
                {
                    formatter.Serialize(fs, list);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        // загрузка с проверкой наличия файла, чтобы программа не падала
        // если файла нет — возвращаем пустой список, чтобы не было NullReferenceException
        static List<T> LoadList<T>(string filePath)
        {
            if (!File.Exists(filePath))
                return new List<T>();

            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    return (List<T>)formatter.Deserialize(fs);
                }
            }
            catch
            {
                // если файл битый, просто возвращаем пустой список, чтобы программа продолжила работу
                // это лучше, чем вылет с ошибкой на ровном месте
                return new List<T>();
            }
        }

        // ДОБАВЛЕНИЕ ДАННЫХ 

        // добавление клиента — без сложной валидации, чтобы не усложнять код
        // в учебном проекте это допустимо
        static void AddMember()
        {
            List<Member> members = LoadList<Member>(memberFile); // загружаем существующих клиентов
            Console.Write("Введите ID клиента: ");
            int id = int.Parse(Console.ReadLine()); // здесь может быть исключение, если введут буквы, но для учебного проекта сойдёт
            Console.Write("Фамилия: ");
            string lastName = Console.ReadLine();
            Console.Write("Имя: ");
            string firstName = Console.ReadLine();
            Console.Write("Телефон: ");
            string phone = Console.ReadLine();
            Console.Write("Дата регистрации (ДД.ММ.ГГГГ): ");
            string regDate = Console.ReadLine();

            members.Add(new Member(id, lastName, firstName, phone, regDate)); // добавляем нового
            SaveList<Member>(members, memberFile); // сохраняем обратно в файл
            Console.WriteLine("Клиент добавлен!");
        }

        static void AddTrainer()
        {
            List<Trainer> trainers = LoadList<Trainer>(trainerFile);
            Console.Write("Введите ID тренера: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Фамилия: ");
            string lastName = Console.ReadLine();
            Console.Write("Имя: ");
            string firstName = Console.ReadLine();
            Console.Write("Специализация: ");
            string spec = Console.ReadLine();
            Console.Write("Стаж (лет): ");
            int exp = int.Parse(Console.ReadLine());

            trainers.Add(new Trainer(id, lastName, firstName, spec, exp));
            SaveList<Trainer>(trainers, trainerFile);
            Console.WriteLine("Тренер добавлен!");
        }

        static void AddWorkout()
        {
            List<Workout> workouts = LoadList<Workout>(workoutFile);
            Console.Write("Введите ID тренировки: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("ID клиента: ");
            int memberId = int.Parse(Console.ReadLine());
            Console.Write("ID тренера: ");
            int trainerId = int.Parse(Console.ReadLine());
            Console.Write("Дата тренировки (ДД.ММ.ГГГГ): ");
            string date = Console.ReadLine();
            Console.Write("Продолжительность (минут): ");
            int duration = int.Parse(Console.ReadLine());

            workouts.Add(new Workout(id, memberId, trainerId, date, duration));
            SaveList<Workout>(workouts, workoutFile);
            Console.WriteLine("Тренировка записана!");
        }

        // ОТОБРАЖЕНИЕ ДАННЫХ

        // здесь подбирал ширину колонок вручную, чтобы таблица красиво выглядела
        // -8 значит, что поле занимает 8 символов и выравнивается по левому краю
        // new string('-', 75) — рисует линию из 75 дефисов
        static void ShowAllMembers()
        {
            List<Member> members = LoadList<Member>(memberFile);
            if (members.Count == 0)
            {
                Console.WriteLine("Нет клиентов.");
                return;
            }
            Console.WriteLine("\n========== КЛИЕНТЫ ==========");
            Console.WriteLine($"{"ID",-8} {"Фамилия",-20} {"Имя",-20} {"Телефон",-15} {"Дата рег.",-12}");
            Console.WriteLine(new string('-', 75));
            foreach (var m in members)
            {
                Console.WriteLine($"{m.Id,-8} {m.LastName,-20} {m.FirstName,-20} {m.Phone,-15} {m.RegDate,-12}");
            }
        }

        static void ShowAllTrainers()
        {
            List<Trainer> trainers = LoadList<Trainer>(trainerFile);
            if (trainers.Count == 0)
            {
                Console.WriteLine("Нет тренеров.");
                return;
            }
            Console.WriteLine("\n========== ТРЕНЕРЫ ==========");
            Console.WriteLine($"{"ID",-8} {"Фамилия",-20} {"Имя",-20} {"Специализация",-30} {"Стаж",-6}");
            Console.WriteLine(new string('-', 84));
            foreach (var t in trainers)
            {
                Console.WriteLine($"{t.Id,-8} {t.LastName,-20} {t.FirstName,-20} {t.Specialization,-30} {t.Experience,-6}");
            }
        }

        static void ShowAllWorkouts()
        {
            List<Workout> workouts = LoadList<Workout>(workoutFile);
            if (workouts.Count == 0)
            {
                Console.WriteLine("Нет тренировок.");
                return;
            }
            Console.WriteLine("\n========== ТРЕНИРОВКИ ==========");
            Console.WriteLine($"{"ID",-8} {"ID клиента",-12} {"ID тренера",-12} {"Дата",-14} {"Мин.",-8}");
            Console.WriteLine(new string('-', 54));
            foreach (var w in workouts)
            {
                Console.WriteLine($"{w.Id,-8} {w.MemberId,-12} {w.TrainerId,-12} {w.WorkoutDate,-14} {w.Duration,-8}");
            }
        }

        // УДАЛЕНИЕ

        static void DeleteMemberById()
        {
            List<Member> members = LoadList<Member>(memberFile);
            Console.Write("Введите ID клиента для удаления: ");
            int id = int.Parse(Console.ReadLine());
            // RemoveAll удаляет все элементы, удовлетворяющие условию, и возвращает количество удалённых
            int removed = members.RemoveAll(m => m.Id == id);
            if (removed > 0)
            {
                SaveList<Member>(members, memberFile);
                Console.WriteLine("Клиент удалён.");
            }
            else
            {
                Console.WriteLine("Не найдено.");
            }
        }

        //  ПОИСК (МИНИМУМ 3 КРИТЕРИЯ)

        // здесь я переделал поиск, теперь он не чувствителен к регистру букв
        // преподаватель сказала исправить, я добавил StringComparison.OrdinalIgnoreCase
        static void SearchSystem()
        {
            Console.WriteLine("\n===== ПОИСК =====");
            Console.WriteLine("1. Найти клиента по фамилии");
            Console.WriteLine("2. Найти тренера по специализации");
            Console.WriteLine("3. Найти тренировки по дате");
            Console.WriteLine("4. Найти тренера со стажем больше N лет");
            Console.Write("Выберите критерий: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1: // поиск клиента по фамилии (без учёта регистра)
                    Console.Write("Введите фамилию: ");
                    string last = Console.ReadLine();
                    var members = LoadList<Member>(memberFile);
                    // исправлено: теперь ищет без учёта регистра
                    // Equals с OrdinalIgnoreCase — это правильный способ
                    var foundMembers = members.Where(m => m.LastName.Equals(last, StringComparison.OrdinalIgnoreCase));
                    foreach (var m in foundMembers)
                        Console.WriteLine($"{m.LastName} {m.FirstName}, тел. {m.Phone}");
                    if (!foundMembers.Any()) Console.WriteLine("Не найдено.");
                    break;
                case 2: // поиск тренера по специализации (частичное совпадение, без учёта регистра)
                    Console.Write("Специализация: ");
                    string spec = Console.ReadLine();
                    var trainers = LoadList<Trainer>(trainerFile);
                    // исправлено: ищет частично и без учёта регистра
                    // ToLower() — костыль, но работает. В идеале надо StringComparison, но Contains его не поддерживает
                    var foundTrainers = trainers.Where(t => t.Specialization.ToLower().Contains(spec.ToLower()));
                    foreach (var t in foundTrainers)
                        Console.WriteLine($"{t.LastName} {t.FirstName} — {t.Specialization}");
                    if (!foundTrainers.Any()) Console.WriteLine("Не найдено.");
                    break;
                case 3: // поиск тренировок по точной дате
                    Console.Write("Дата (ДД.ММ.ГГГГ): ");
                    string date = Console.ReadLine();
                    var workouts = LoadList<Workout>(workoutFile);
                    var foundWorkouts = workouts.Where(w => w.WorkoutDate == date);
                    foreach (var w in foundWorkouts)
                        Console.WriteLine($"Тренировка ID {w.Id}, клиент {w.MemberId}, тренер {w.TrainerId}, {w.Duration} мин");
                    if (!foundWorkouts.Any()) Console.WriteLine("Не найдено.");
                    break;
                case 4: // поиск тренеров со стажем больше N
                    Console.Write("Минимальный стаж: ");
                    int minExp = int.Parse(Console.ReadLine());
                    var trainers2 = LoadList<Trainer>(trainerFile);
                    var expTrainers = trainers2.Where(t => t.Experience >= minExp);
                    foreach (var t in expTrainers)
                        Console.WriteLine($"{t.LastName} {t.FirstName} — стаж {t.Experience} лет");
                    if (!expTrainers.Any()) Console.WriteLine("Не найдено.");
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }

        // СТАТИСТИКА (МИНИМУМ 2 ХАРАКТЕРИСТИКИ)

        // статистика — минимальный набор, как требовалось в задании
        static void SummaryStats()
        {
            var trainers = LoadList<Trainer>(trainerFile);
            var workouts = LoadList<Workout>(workoutFile);
            var members = LoadList<Member>(memberFile);

            Console.WriteLine("\n========== СТАТИСТИКА ==========");

            // 1-я характеристика: количество тренировок у каждого тренера
            Console.WriteLine("\n1) Количество тренировок у тренеров:");
            foreach (var t in trainers)
            {
                int count = workouts.Count(w => w.TrainerId == t.Id);
                Console.WriteLine($"{t.LastName} {t.FirstName}: {count} тренировок");
            }

            // 2-я характеристика: общее время тренировок конкретного клиента (в минутах)
            if (members.Count > 0)
            {
                Console.Write("\n2) Введите ID клиента для подсчёта его минут: ");
                int memberId = int.Parse(Console.ReadLine());
                int totalMinutes = workouts.Where(w => w.MemberId == memberId).Sum(w => w.Duration);
                Console.WriteLine($"Общее время тренировок: {totalMinutes} минут.");
            }
        }

        //  СОРТИРОВКА

        // сортируем тренеров по разным критериям (фамилия, стаж) в прямом и обратном порядке
        static void SortTrainers()
        {
            List<Trainer> trainers = LoadList<Trainer>(trainerFile);
            if (trainers.Count == 0)
            {
                Console.WriteLine("Нет тренеров.");
                return;
            }

            Console.WriteLine("\nСортировка тренеров:");
            Console.WriteLine("1. По фамилии (А-Я)");
            Console.WriteLine("2. По фамилии (Я-А)");
            Console.WriteLine("3. По стажу (возрастание)");
            Console.WriteLine("4. По стажу (убывание)");
            Console.Write("Выбор: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    trainers = trainers.OrderBy(t => t.LastName).ToList();
                    break;
                case 2:
                    trainers = trainers.OrderByDescending(t => t.LastName).ToList();
                    break;
                case 3:
                    trainers = trainers.OrderBy(t => t.Experience).ToList();
                    break;
                case 4:
                    trainers = trainers.OrderByDescending(t => t.Experience).ToList();
                    break;
                default:
                    Console.WriteLine("Неверно.");
                    return;
            }

            SaveList<Trainer>(trainers, trainerFile);
            Console.WriteLine("Сортировка выполнена!");
            ShowAllTrainers(); // показываем результат
        }

        // ГЛАВНОЕ МЕНЮ 

        static void MainMenu()
        {
            int choice;
            do
            {
                // меню сделал без замысловатой графики, чтобы в любой консоли работало
                // псевдографика типа ┌──┐ иногда не отображается, поэтому использую обычные символы
                Console.WriteLine("\n==================================================");
                Console.WriteLine("   СПОРТИВНЫЙ КЛУБ - СИСТЕМА УЧЁТА ТРЕНИРОВОК");
                Console.WriteLine("==================================================");
                Console.WriteLine("1. Добавить данные");
                Console.WriteLine("2. Показать все данные (таблицы)");
                Console.WriteLine("3. Удалить клиента по ID");
                Console.WriteLine("4. Поиск (по фамилии, специализации, дате, стажу)");
                Console.WriteLine("5. Суммарные характеристики (статистика)");
                Console.WriteLine("6. Сортировка тренеров");
                Console.WriteLine("0. Выход");
                Console.Write("Ваш выбор: ");
                choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        // подменю для добавления: выбираем, кого добавлять
                        Console.Write("1 - Клиент, 2 - Тренер, 3 - Тренировка: ");
                        int sub = int.Parse(Console.ReadLine());
                        if (sub == 1) AddMember();
                        else if (sub == 2) AddTrainer();
                        else if (sub == 3) AddWorkout();
                        else Console.WriteLine("Неверно.");
                        break;
                    case 2:
                        // показываем все три таблицы
                        ShowAllMembers();
                        ShowAllTrainers();
                        ShowAllWorkouts();
                        break;
                    case 3:
                        DeleteMemberById();
                        break;
                    case 4:
                        SearchSystem();
                        break;
                    case 5:
                        SummaryStats();
                        break;
                    case 6:
                        SortTrainers();
                        break;
                    case 0:
                        Console.WriteLine("Выход. Данные сохранены в .bin файлах.");
                        break;
                    default:
                        Console.WriteLine("Ошибка. Попробуйте снова.");
                        break;
                }
            } while (choice != 0);
        }

        //  ТОЧКА ВХОДА

        static void Main(string[] args)
        {
            // чтобы русские буквы нормально отображались в консоли (иначе кракозябры)
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.Title = "Спортивный клуб - Курсовой проект"; // заголовок окна консоли
            
            // приветствие и информация о файлах
            Console.WriteLine("\n=== Курсовой проект: Спортивный клуб (C#) ===");
            Console.WriteLine($"Файлы данных:\n{memberFile}\n{trainerFile}\n{workoutFile}");
            Console.WriteLine("\nЕсли файлов нет — создадутся автоматически при добавлении.");
            Console.WriteLine("Нажмите любую клавишу для входа в систему...");
            Console.ReadKey(); // ждём нажатия, чтобы пользователь успел прочитать
            
            MainMenu(); // запускаем главное меню
        }
    }
}
