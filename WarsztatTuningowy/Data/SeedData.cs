using Microsoft.AspNetCore.Identity;
using WarsztatTuningowy.Models.Domain;
using WarsztatTuningowy.Models.Enums;

namespace WarsztatTuningowy.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Owner", "Mechanic", "Storekeeper"];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            await CreateUser(userManager,
                "wlasciciel@warsztat.pl", "Admin123!", "Owner");
            await CreateUser(userManager,
                "mechanik1@warsztat.pl", "Admin123!", "Mechanic");
            await CreateUser(userManager,
                "mechanik2@warsztat.pl", "Admin123!", "Mechanic");
            await CreateUser(userManager,
                "mechanik3@warsztat.pl", "Admin123!", "Mechanic");
            await CreateUser(userManager,
                "magazynier@warsztat.pl", "Admin123!", "Storekeeper");

            if (context.Employees.Any()) return;

            var wlascicielUser = await userManager
                .FindByEmailAsync("wlasciciel@warsztat.pl");
            var mechanik1User = await userManager
                .FindByEmailAsync("mechanik1@warsztat.pl");
            var mechanik2User = await userManager
                .FindByEmailAsync("mechanik2@warsztat.pl");
            var mechanik3User = await userManager
                .FindByEmailAsync("mechanik3@warsztat.pl");
            var magazynierUser = await userManager
                .FindByEmailAsync("magazynier@warsztat.pl");

            var employees = new List<Employee>
            {
                new() {
                    FullName = "Marek Kowalski",
                    Role = EmployeeRole.Owner,
                    HourlyRateInternal = 80,
                    HourlyRateClient = 300,
                    UserId = wlascicielUser?.Id
                },
                new() {
                    FullName = "Piotr Nowak",
                    Role = EmployeeRole.Mechanic,
                    HourlyRateInternal = 60,
                    HourlyRateClient = 250,
                    UserId = mechanik1User?.Id
                },
                new() {
                    FullName = "Damian Kowalczyk",
                    Role = EmployeeRole.Mechanic,
                    HourlyRateInternal = 55,
                    HourlyRateClient = 230,
                    UserId = mechanik2User?.Id
                },
                new() {
                    FullName = "Łukasz Grabowski",
                    Role = EmployeeRole.Mechanic,
                    HourlyRateInternal = 65,
                    HourlyRateClient = 270,
                    UserId = mechanik3User?.Id
                },
                new() {
                    FullName = "Tomasz Wiśniewski",
                    Role = EmployeeRole.Storekeeper,
                    HourlyRateInternal = 40,
                    HourlyRateClient = 0,
                    UserId = magazynierUser?.Id
                }
            };
            context.Employees.AddRange(employees);
            await context.SaveChangesAsync();

            var owner = employees[0];
            var mech1 = employees[1];
            var mech2 = employees[2];
            var mech3 = employees[3];

            var clients = new List<Client>
            {
                new() {
                    FullName = "Adam Zieliński",
                    Phone = "601 234 567",
                    Email = "adam.zielinski@gmail.com",
                    CreatedAt = DateTime.Now.AddMonths(-8)
                },
                new() {
                    FullName = "Bartosz Wójcik",
                    Phone = "602 345 678",
                    Email = "b.wojcik@gmail.com",
                    CreatedAt = DateTime.Now.AddMonths(-6)
                },
                new() {
                    FullName = "Krzysztof Lewandowski",
                    Phone = "603 456 789",
                    Email = "k.lewandowski@wp.pl",
                    CreatedAt = DateTime.Now.AddMonths(-4)
                },
                new() {
                    FullName = "Monika Dąbrowska",
                    Phone = "604 567 890",
                    Email = "m.dabrowska@onet.pl",
                    CreatedAt = DateTime.Now.AddMonths(-2)
                },
                new() {
                    FullName = "Rafał Szymański",
                    Phone = "605 678 901",
                    Email = "r.szymanski@gmail.com",
                    CreatedAt = DateTime.Now.AddMonths(-1)
                },
                new() {
                    FullName = "Michał Kamiński",
                    Phone = "606 789 012",
                    Email = "m.kaminski@gmail.com",
                    CreatedAt = DateTime.Now.AddDays(-5)
                }
            };
            context.Clients.AddRange(clients);
            await context.SaveChangesAsync();

            var vehicles = new List<Vehicle>
            {
                new() {
                    VIN = "WBA3A5G59DNP26082",
                    Brand = "BMW", Model = "M3 Competition",
                    Year = 2021,
                    EngineType = "3.0 TwinPower Turbo 510KM",
                    ECUType = "Bosch MG1CS002",
                    ClientId = clients[0].Id
                },
                new() {
                    VIN = "WAUZZZ8K9BA012345",
                    Brand = "Audi", Model = "RS3 Sportback",
                    Year = 2022,
                    EngineType = "2.5 TFSI 400KM",
                    ECUType = "Bosch MED17.1",
                    ClientId = clients[1].Id
                },
                new() {
                    VIN = "WP0ZZZ99ZTS123456",
                    Brand = "Porsche", Model = "718 Cayman GTS",
                    Year = 2020,
                    EngineType = "4.0 Boxer 400KM",
                    ECUType = "Bosch ME17.8",
                    ClientId = clients[2].Id
                },
                new() {
                    VIN = "WDB2130001A123456",
                    Brand = "Mercedes", Model = "C63 AMG",
                    Year = 2019,
                    EngineType = "4.0 V8 Biturbo 476KM",
                    ECUType = "Bosch ME17.7",
                    ClientId = clients[3].Id
                },
                new() {
                    VIN = "WF0XXXGAJX5E12345",
                    Brand = "Ford", Model = "Focus ST",
                    Year = 2022,
                    EngineType = "2.3 EcoBoost 280KM",
                    ECUType = "Bosch MED17.2",
                    ClientId = clients[4].Id
                },
                new() {
                    VIN = "WBA5R71000E123456",
                    Brand = "BMW", Model = "335i xDrive",
                    Year = 2018,
                    EngineType = "3.0 TwinPower Turbo 306KM",
                    ECUType = "Bosch MSD80",
                    ClientId = clients[0].Id
                },
                new() {
                    VIN = "WVWZZZAUZKW123456",
                    Brand = "Volkswagen", Model = "Golf R",
                    Year = 2021,
                    EngineType = "2.0 TSI 320KM",
                    ECUType = "Bosch MG1CS011",
                    ClientId = clients[5].Id
                }
            };
            context.Vehicles.AddRange(vehicles);
            await context.SaveChangesAsync();

            var parts = new List<Part>
            {
                new() {
                    Name = "Intercooler Wagner Tuning EVO2",
                    WholesalePrice = 2200,
                    RetailPrice = 3100,
                    Stock = 2,
                    MinStock = 1,
                    SupplierName = "Wagner Tuning PL",
                    IsStockPart = false
                },
                new() {
                    Name = "Wydech Milltek Sport Non-Res",
                    WholesalePrice = 3800,
                    RetailPrice = 5200,
                    Stock = 1,
                    MinStock = 1,
                    SupplierName = "Milltek Sport",
                    IsStockPart = false
                },
                new() {
                    Name = "Filtr powietrza K&N 57i",
                    WholesalePrice = 320,
                    RetailPrice = 490,
                    Stock = 5,
                    MinStock = 2,
                    SupplierName = "K&N Filters",
                    IsStockPart = true
                },
                new() {
                    Name = "Olej Motul 5W40 8100 X-Cess 5L",
                    WholesalePrice = 110,
                    RetailPrice = 160,
                    Stock = 18,
                    MinStock = 5,
                    SupplierName = "Motul Polska",
                    IsStockPart = true
                },
                new() {
                    Name = "Downpipe 3\" bez katalizatora",
                    WholesalePrice = 890,
                    RetailPrice = 1350,
                    Stock = 0,
                    MinStock = 1,
                    SupplierName = "Fabspeed Motorsport",
                    IsStockPart = false
                },
                new() {
                    Name = "Turbo Garrett GTX3076R Gen2",
                    WholesalePrice = 7500,
                    RetailPrice = 9800,
                    Stock = 1,
                    MinStock = 1,
                    SupplierName = "Garrett Motion",
                    IsStockPart = false
                },
                new() {
                    Name = "Wtryskiwacze Bosch 1000cc (kpl. 6)",
                    WholesalePrice = 1800,
                    RetailPrice = 2600,
                    Stock = 1,
                    MinStock = 2,
                    SupplierName = "Bosch Motorsport",
                    IsStockPart = false
                },
                new() {
                    Name = "Świece NGK Racing R7437-9",
                    WholesalePrice = 420,
                    RetailPrice = 690,
                    Stock = 20,
                    MinStock = 4,
                    SupplierName = "NGK Motorsport",
                    IsStockPart = true
                },
                new() {
                    Name = "Klocki hamulcowe Ferodo DS2500",
                    WholesalePrice = 760,
                    RetailPrice = 1150,
                    Stock = 3,
                    MinStock = 2,
                    SupplierName = "Ferodo Racing",
                    IsStockPart = true
                },
                new() {
                    Name = "Przewody hamulcowe HEL Performance",
                    WholesalePrice = 360,
                    RetailPrice = 590,
                    Stock = 2,
                    MinStock = 1,
                    SupplierName = "HEL Performance",
                    IsStockPart = true
                },
                new() {
                    Name = "Pompa paliwa Walbro 450",
                    WholesalePrice = 520,
                    RetailPrice = 820,
                    Stock = 2,
                    MinStock = 1,
                    SupplierName = "Walbro",
                    IsStockPart = true
                },
                new() {
                    Name = "Zestaw poliuretanów Strongflex",
                    WholesalePrice = 980,
                    RetailPrice = 1450,
                    Stock = 1,
                    MinStock = 1,
                    SupplierName = "Strongflex",
                    IsStockPart = false
                },
                new() {
                    Name = "Folia PPF matowa dach + słupki",
                    WholesalePrice = 700,
                    RetailPrice = 1300,
                    Stock = 1,
                    MinStock = 1,
                    SupplierName = "3M Automotive",
                    IsStockPart = false
                }
            };
            context.Parts.AddRange(parts);
            await context.SaveChangesAsync();

            var orders = new List<Order>
            {
                new() {
                    TuningGoal = TuningGoal.Stage2 | TuningGoal.Mechanical,
                    Status = OrderStatus.Received,
                    EstimatedHours = 12,
                    DepositPaid = true,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    VehicleId = vehicles[0].Id,
                    DefaultMechanicId = mech1.Id,
                    ModificationScopeAccepted = true,
                    LegalConsequencesAccepted = true,
                    WarrantyLossAccepted = true,
                    StatementAcceptedAt = DateTime.Now.AddDays(-1).AddHours(1),
                    StatementAcceptedBy = clients[0].FullName
                },
                new() {
                    TuningGoal = TuningGoal.Stage1,
                    Status = OrderStatus.Diagnosis,
                    EstimatedHours = 4,
                    DepositPaid = true,
                    CreatedAt = DateTime.Now.AddDays(-3),
                    VehicleId = vehicles[1].Id,
                    DefaultMechanicId = mech2.Id,
                    ModificationScopeAccepted = true,
                    LegalConsequencesAccepted = true,
                    WarrantyLossAccepted = true,
                    StatementAcceptedAt = DateTime.Now.AddDays(-3).AddHours(2),
                    StatementAcceptedBy = clients[1].FullName
                },
                new() {
                    TuningGoal = TuningGoal.Stage3 | TuningGoal.Mechanical,
                    Status = OrderStatus.InProgress,
                    EstimatedHours = 20,
                    DepositPaid = true,
                    CreatedAt = DateTime.Now.AddDays(-7),
                    VehicleId = vehicles[2].Id,
                    DefaultMechanicId = mech3.Id,
                    ModificationScopeAccepted = true,
                    LegalConsequencesAccepted = true,
                    WarrantyLossAccepted = true,
                    StatementAcceptedAt = DateTime.Now.AddDays(-7).AddHours(1),
                    StatementAcceptedBy = clients[2].FullName
                },
                new() {
                    TuningGoal = TuningGoal.Visual,
                    Status = OrderStatus.QualityCheck,
                    EstimatedHours = 6,
                    DepositPaid = true,
                    CreatedAt = DateTime.Now.AddDays(-10),
                    VehicleId = vehicles[3].Id,
                    DefaultMechanicId = mech1.Id,
                    ModificationScopeAccepted = true,
                    LegalConsequencesAccepted = true,
                    WarrantyLossAccepted = true,
                    StatementAcceptedAt = DateTime.Now.AddDays(-10).AddHours(3),
                    StatementAcceptedBy = clients[3].FullName
                },
                new() {
                    TuningGoal = TuningGoal.Stage1 | TuningGoal.Mechanical,
                    Status = OrderStatus.Completed,
                    EstimatedHours = 8,
                    DepositPaid = true,
                    CreatedAt = DateTime.Now.AddDays(-20),
                    VehicleId = vehicles[4].Id,
                    DefaultMechanicId = mech2.Id,
                    ModificationScopeAccepted = true,
                    LegalConsequencesAccepted = true,
                    WarrantyLossAccepted = true,
                    StatementAcceptedAt = DateTime.Now.AddDays(-20).AddHours(1),
                    StatementAcceptedBy = clients[4].FullName
                },
                new() {
                    TuningGoal = TuningGoal.Stage2,
                    Status = OrderStatus.InProgress,
                    EstimatedHours = 6,
                    DepositPaid = true,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    VehicleId = vehicles[5].Id,
                    DefaultMechanicId = mech1.Id
                },
                new() {
                    TuningGoal = TuningGoal.Stage1,
                    Status = OrderStatus.Received,
                    EstimatedHours = 5,
                    DepositPaid = false,
                    CreatedAt = DateTime.Now,
                    VehicleId = vehicles[6].Id,
                    DefaultMechanicId = mech2.Id
                }
            };
            context.Orders.AddRange(orders);
            await context.SaveChangesAsync();

            var serviceTasks = new List<ServiceTask>
            {
                new() {
                    Name = "Pre-scan DTC i zapis logów bazowych",
                    OrderId = orders[0].Id,
                    AssignedEmployeeId = mech1.Id,
                    StartTime = DateTime.Now.AddHours(-7),
                    EndTime = DateTime.Now.AddHours(-6),
                    PausedMinutes = 0,
                    Notes = $"[{DateTime.Now.AddHours(-6):dd.MM.yyyy HH:mm}] " +
                            "Brak aktywnych błędów. Logi bazowe zapisane."
                },
                new() {
                    Name = "Demontaż zderzaka i przygotowanie pod intercooler",
                    OrderId = orders[0].Id,
                    AssignedEmployeeId = mech1.Id
                },
                new() {
                    Name = "Montaż intercoolera Wagner EVO2",
                    OrderId = orders[0].Id,
                    AssignedEmployeeId = mech1.Id
                },
                new() {
                    Name = "Diagnostyka ECU i odczyt kodów DTC",
                    OrderId = orders[1].Id,
                    AssignedEmployeeId = mech2.Id,
                    StartTime = DateTime.Now.AddDays(-2).AddHours(-4),
                    EndTime = DateTime.Now.AddDays(-2).AddHours(-2),
                    PausedMinutes = 10,
                    Notes = $"[{DateTime.Now.AddDays(-2).AddHours(-2):dd.MM.yyyy HH:mm}] " +
                            "Wykryto sporadyczny błąd sondy lambda. Do obserwacji."
                },
                new() {
                    Name = "Kontrola szczelności dolotu",
                    OrderId = orders[1].Id,
                    AssignedEmployeeId = mech2.Id,
                    StartTime = DateTime.Now.AddDays(-1).AddHours(-5),
                    EndTime = DateTime.Now.AddDays(-1).AddHours(-4),
                    PausedMinutes = 0,
                    Notes = $"[{DateTime.Now.AddDays(-1).AddHours(-4):dd.MM.yyyy HH:mm}] " +
                            "Dolot szczelny, przepływomierz pracuje stabilnie."
                },
                new() {
                    Name = "Przygotowanie mapy Stage 1",
                    OrderId = orders[1].Id,
                    AssignedEmployeeId = mech2.Id
                },
                new() {
                    Name = "Wymiana turbo na Garrett GTX3076R",
                    OrderId = orders[2].Id,
                    AssignedEmployeeId = mech3.Id,
                    StartTime = DateTime.Now.AddHours(-3),
                    PausedMinutes = 15,
                    Notes = $"[{DateTime.Now.AddHours(-2):dd.MM.yyyy HH:mm}] " +
                            "Turbo zdemontowane. Czekam na nowy uszczelniacz."
                },
                new() {
                    Name = "Montaż wtryskiwaczy 1000cc",
                    OrderId = orders[2].Id,
                    AssignedEmployeeId = mech3.Id,
                    StartTime = DateTime.Now.AddDays(-2),
                    EndTime = DateTime.Now.AddDays(-2).AddHours(4),
                    PausedMinutes = 30,
                    Notes = $"[{DateTime.Now.AddDays(-2):dd.MM.yyyy HH:mm}] " +
                            "Wtryskiwacze zamontowane. Moment dokręcenia 20Nm."
                },
                new() {
                    Name = "Montaż pompy paliwa Walbro 450",
                    OrderId = orders[2].Id,
                    AssignedEmployeeId = mech3.Id,
                    StartTime = DateTime.Now.AddDays(-1).AddHours(-5),
                    EndTime = DateTime.Now.AddDays(-1).AddHours(-2),
                    PausedMinutes = 20,
                    Notes = $"[{DateTime.Now.AddDays(-1).AddHours(-2):dd.MM.yyyy HH:mm}] " +
                            "Ciśnienie paliwa po montażu stabilne."
                },
                new() {
                    Name = "Strojenie na hamowni Stage 3",
                    OrderId = orders[2].Id,
                    AssignedEmployeeId = mech3.Id,
                    StartTime = DateTime.Now.AddHours(-5),
                    PausedMinutes = 10,
                    Notes = $"[{DateTime.Now.AddHours(-3):dd.MM.yyyy HH:mm}] " +
                            "Trwa korekta zapłonu i dawki paliwa pod wyższe doładowanie."
                },
                new() {
                    Name = "Oklejanie dachu folią matową",
                    OrderId = orders[3].Id,
                    AssignedEmployeeId = mech1.Id,
                    StartTime = DateTime.Now.AddDays(-3),
                    EndTime = DateTime.Now.AddDays(-3).AddHours(5),
                    PausedMinutes = 0
                },
                new() {
                    Name = "Detailing przygotowawczy i odtłuszczenie lakieru",
                    OrderId = orders[3].Id,
                    AssignedEmployeeId = mech1.Id,
                    StartTime = DateTime.Now.AddDays(-4).AddHours(-3),
                    EndTime = DateTime.Now.AddDays(-4).AddHours(-1),
                    PausedMinutes = 0
                },
                new() {
                    Name = "Kontrola jakości aplikacji folii",
                    OrderId = orders[3].Id,
                    AssignedEmployeeId = mech1.Id,
                    StartTime = DateTime.Now.AddHours(-4),
                    EndTime = DateTime.Now.AddHours(-3),
                    PausedMinutes = 0,
                    Notes = $"[{DateTime.Now.AddHours(-3):dd.MM.yyyy HH:mm}] " +
                            "Brak pęcherzy, krawędzie zabezpieczone."
                },
                new() {
                    Name = "Chiptuning Stage 1 + downpipe",
                    OrderId = orders[4].Id,
                    AssignedEmployeeId = mech2.Id,
                    StartTime = DateTime.Now.AddDays(-20),
                    EndTime = DateTime.Now.AddDays(-20).AddHours(6),
                    PausedMinutes = 45,
                    Notes = $"[{DateTime.Now.AddDays(-20):dd.MM.yyyy HH:mm}] " +
                            "Mapa Stage 1 załadowana. Wyniki hamowni: +48KM / +72Nm."
                },
                new() {
                    Name = "Wymiana oleju i świec po strojeniu",
                    OrderId = orders[4].Id,
                    AssignedEmployeeId = mech2.Id,
                    StartTime = DateTime.Now.AddDays(-19).AddHours(-4),
                    EndTime = DateTime.Now.AddDays(-19).AddHours(-2),
                    PausedMinutes = 0
                },
                new() {
                    Name = "Jazda próbna i kontrola logów",
                    OrderId = orders[4].Id,
                    AssignedEmployeeId = mech2.Id,
                    StartTime = DateTime.Now.AddDays(-19).AddHours(-2),
                    EndTime = DateTime.Now.AddDays(-19),
                    PausedMinutes = 0,
                    Notes = $"[{DateTime.Now.AddDays(-19):dd.MM.yyyy HH:mm}] " +
                            "Auto wydane klientowi. Brak knock correction."
                },
                new() {
                    Name = "Flashowanie ECU Stage 2",
                    OrderId = orders[5].Id,
                    AssignedEmployeeId = mech1.Id,
                    StartTime = DateTime.Now.AddHours(-8),
                    EndTime = DateTime.Now.AddHours(-3),
                    PausedMinutes = 30,
                    Notes = $"[{DateTime.Now.AddHours(-3):dd.MM.yyyy HH:mm}] " +
                            "Mapa wgrana, auto wymaga ponownych logów pod obciążeniem."
                },
                new() {
                    Name = "Montaż świec i kontrola zapłonu",
                    OrderId = orders[5].Id,
                    AssignedEmployeeId = mech1.Id,
                    StartTime = DateTime.Now.AddHours(-2),
                    PausedMinutes = 0,
                    Notes = $"[{DateTime.Now.AddHours(-1):dd.MM.yyyy HH:mm}] " +
                            "W trakcie kontroli cewek zapłonowych."
                }
            };
            context.ServiceTasks.AddRange(serviceTasks);
            await context.SaveChangesAsync();

            var orderParts = new List<OrderPart>
            {
                new() {
                    OrderId = orders[0].Id,
                    PartId = parts[0].Id,
                    Quantity = 1,
                    IsUsed = false,
                    IsVinLocked = true,
                    LockedVin = vehicles[0].VIN
                },
                new() {
                    OrderId = orders[0].Id,
                    PartId = parts[2].Id,
                    Quantity = 1,
                    IsUsed = false
                },
                new() {
                    OrderId = orders[0].Id,
                    PartId = parts[7].Id,
                    Quantity = 6,
                    IsUsed = false
                },
                new() {
                    OrderId = orders[0].Id,
                    PartId = parts[8].Id,
                    Quantity = 1,
                    IsUsed = false
                },

                new() {
                    OrderId = orders[1].Id,
                    PartId = parts[2].Id,
                    Quantity = 1,
                    IsUsed = false
                },
                new() {
                    OrderId = orders[1].Id,
                    PartId = parts[7].Id,
                    Quantity = 5,
                    IsUsed = false
                },
                new() {
                    OrderId = orders[1].Id,
                    PartId = parts[9].Id,
                    Quantity = 1,
                    IsUsed = false
                },

                new() {
                    OrderId = orders[2].Id,
                    PartId = parts[5].Id,
                    Quantity = 1,
                    IsUsed = true,
                    IsVinLocked = true,
                    LockedVin = vehicles[2].VIN
                },
                new() {
                    OrderId = orders[2].Id,
                    PartId = parts[6].Id,
                    Quantity = 1,
                    IsUsed = true
                },
                new() {
                    OrderId = orders[2].Id,
                    PartId = parts[10].Id,
                    Quantity = 1,
                    IsUsed = true
                },
                new() {
                    OrderId = orders[2].Id,
                    PartId = parts[7].Id,
                    Quantity = 6,
                    IsUsed = true
                },

                new() {
                    OrderId = orders[3].Id,
                    PartId = parts[12].Id,
                    Quantity = 1,
                    IsUsed = true,
                    IsVinLocked = true,
                    LockedVin = vehicles[3].VIN
                },

                new() {
                    OrderId = orders[4].Id,
                    PartId = parts[3].Id,
                    Quantity = 2,
                    IsUsed = true
                },
                new() {
                    OrderId = orders[4].Id,
                    PartId = parts[2].Id,
                    Quantity = 1,
                    IsUsed = true
                },
                new() {
                    OrderId = orders[4].Id,
                    PartId = parts[4].Id,
                    Quantity = 1,
                    IsUsed = true
                },
                new() {
                    OrderId = orders[4].Id,
                    PartId = parts[7].Id,
                    Quantity = 4,
                    IsUsed = true
                },

                new() {
                    OrderId = orders[5].Id,
                    PartId = parts[7].Id,
                    Quantity = 6,
                    IsUsed = true
                },
                new() {
                    OrderId = orders[5].Id,
                    PartId = parts[3].Id,
                    Quantity = 1,
                    IsUsed = false
                },
                new() {
                    OrderId = orders[5].Id,
                    PartId = parts[1].Id,
                    Quantity = 1,
                    IsUsed = false,
                    IsVinLocked = true,
                    LockedVin = vehicles[5].VIN
                }
            };
            context.OrderParts.AddRange(orderParts);
            await context.SaveChangesAsync();

            var partRequests = new List<PartRequest>
            {
                new() {
                    OrderId = orders[1].Id,
                    PartId = parts[9].Id,
                    Quantity = 1,
                    Notes = "Przewody potrzebne przed finalnym strojeniem RS3.",
                    Status = PartRequestStatus.Ready,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    RequestedByEmployeeId = mech2.Id
                },
                new() {
                    OrderId = orders[2].Id,
                    CustomPartName = "Uszczelniacz turbo Garrett GTX - komplet",
                    Quantity = 1,
                    Notes = "Część jednorazowa pod aktualny montaż turbo.",
                    Status = PartRequestStatus.Pending,
                    CreatedAt = DateTime.Now.AddHours(-6),
                    RequestedByEmployeeId = mech3.Id
                },
                new() {
                    OrderId = orders[5].Id,
                    PartId = parts[4].Id,
                    Quantity = 1,
                    Notes = "Downpipe wymagany do finalizacji Stage 2.",
                    Status = PartRequestStatus.Pending,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    RequestedByEmployeeId = mech1.Id
                },
                new() {
                    OrderId = orders[3].Id,
                    CustomPartName = "Emblematy AMG Black Series - komplet",
                    Quantity = 1,
                    Notes = "Część customowa do zlecenia wizualnego, gotowa do odbioru.",
                    Status = PartRequestStatus.Ready,
                    CreatedAt = DateTime.Now.AddHours(-10),
                    RequestedByEmployeeId = mech1.Id
                },
                new() {
                    OrderId = orders[1].Id,
                    CustomPartName = "Uszczelka downpipe RS3 8Y",
                    Quantity = 1,
                    Notes = "Czekamy na potwierdzenie dostępności u dostawcy.",
                    Status = PartRequestStatus.Ordered,
                    CreatedAt = DateTime.Now.AddDays(-1),
                    RequestedByEmployeeId = mech2.Id
                }
            };
            context.PartRequests.AddRange(partRequests);
            await context.SaveChangesAsync();

            var workstations = context.Workstations.ToList();
            if (workstations.Any())
            {
                var assignments = new List<WorkstationAssignment>
                {
                    new() {
                        OrderId = orders[2].Id,
                        WorkstationId = workstations
                            .First(w => w.Type == WorkstationType.Dyno).Id,
                        AssignedAt = DateTime.Now.AddDays(-7),
                        ReleasedAt = null, // aktywne
                        Notes = "Sesja hamowni Stage 3"
                    },
                    new() {
                        OrderId = orders[1].Id,
                        WorkstationId = workstations
                            .First(w => w.Type == WorkstationType.Diagnostic).Id,
                        AssignedAt = DateTime.Now.AddDays(-3),
                        ReleasedAt = DateTime.Now.AddDays(-2),
                        Notes = "Diagnostyka wstępna RS3"
                    },
                    new() {
                        OrderId = orders[4].Id,
                        WorkstationId = workstations
                            .First(w => w.Type == WorkstationType.Lift).Id,
                        AssignedAt = DateTime.Now.AddDays(-20),
                        ReleasedAt = DateTime.Now.AddDays(-19),
                        Notes = "Montaż downpipe Focus ST"
                    }
                };
                context.WorkstationAssignments.AddRange(assignments);

                var dyno = workstations
                    .First(w => w.Type == WorkstationType.Dyno);
                dyno.Occupy();

                await context.SaveChangesAsync();
            }
        }

        private static async Task CreateUser(
            UserManager<IdentityUser> userManager,
            string email,
            string password,
            string role)
        {
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
