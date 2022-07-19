clear, clc

% Параметры
Delta = 0.2;                    % Временной шаг 
G = 6.67E-11;                   % Гравитационная постоянная
ME = 5.97E+24;                  % Масса Земли
R = 6.371E+6;                   % радиус Земли
Mass_End = 30000;               % Масса полезной части ракеты


%Mass_End = input("Масса полезной части ракеты (кг):");
input_type = input("Ручной ввод ступеней (Y/N):", "s");

if lower(input_type) == 'y'

    Stages_Number = input("Кол-во ступеней: ");

    % Stages(i, j) i - stage number
    % j = 1 -- all mass
    % j = 2 -- fuel mass
    % j = 3 -- velocity
    % j = 4 -- mass loss
    Stages = zeros(round(Stages_Number), 4);

    Stage_Mass = zeros(1, round(Stages_Number));
    Stage_Fuel = zeros(1, round(Stages_Number));
    Stage_Velocity = zeros(1, round(Stages_Number));
    Stage_Loss = zeros(1, round(Stages_Number));
    
    for i=1:Stages_Number
        Stages(i, 1) = input(sprintf("Общая масса ступени %d:", i));
        Stages(i, 2) = input(sprintf("Масса топлива ступени %d:", i));
        Stages(i, 3) = input(sprintf("Скорость газа ступени %d:", i));
        Stages(i, 4) = input(sprintf("Расход топлива ступени %d:", i));
    end

else

    % file structure
    %
    %               full_mass(kg) fuel_mass(kg)   gas_velocity(m/s)   mass_loss(kg/s)
    %     stage(1)      x1              y1              z1                  v1 
    %     stage(2)      x2              y2              z2                  v2 
    %       ...
    %     stage(n)     x_n             y_n              z_n                v_n 


    fileID = fopen('data.txt','r');
    formatSpec = '%d %d %d %d';
    s = [4 Inf];
    Stages = fscanf(fileID,formatSpec,s);
    Stages = Stages';
    [Stages_Number, a] = size(Stages(:,1));

end

for i=1:Stages_Number
    Stages(i, 1) = Stages(i, 1) - Stages(i, 2);
end

time_end = round(max(sum(Stages(:, 2)/Stages(:, 4)) / Delta));
Memory_Allocation = time_end+round(time_end*0.1);

t = zeros(1, Memory_Allocation);
Thrust = zeros(1, Memory_Allocation);
Mass = zeros(1, Memory_Allocation);
Gravity = zeros(1, Memory_Allocation);
F = zeros(1, Memory_Allocation);
A = zeros(1, Memory_Allocation);
V = zeros(1, Memory_Allocation);
x = zeros(1, Memory_Allocation);

V(1) = 0;                      % Начальное значение скорости (м/с)
x(1) = 0.1;                    % Начальное значение высоты (м)
Mass(1) = Mass_End + sum(Stages(:,1)) + sum(Stages(:,2));       % Начальное значение массы (кг)

n = 1;                          % Момент времени 1
stage_n = 1;

grafs = true;
undocks = 0;
while n ~= (Memory_Allocation)        
    n = n+1;                                        % Увеличиваем момент времени
    t(n)= (n-1)*Delta;                              % Высчитываем время                     
    
    if Stages(stage_n, 2) > 0                       % Есть ли топливо в ступени
        dmdt = Stages(stage_n, 4);                  % Задаем скорость уменьшения массы
        u = Stages(stage_n, 3);                     % Задаем скорость истечения газов
    else                            
        dmdt = 0;                                   % Топливо закончилось
        u = 0;
    end

    if dmdt == 0 && undocks ~= Stages_Number        % Если топливо закончилось и еще остались ступени
        Mass(n) = Mass(n-1) - Stages(stage_n, 1);   % Отбрасываем массу ступени
        if stage_n <= (Stages_Number - 1)           % Меняем ступень
            stage_n = stage_n + 1;
        end
        undocks = undocks + 1;
    else                                            
        Stages(stage_n, 2) = Stages(stage_n, 2) - dmdt * Delta; % Топливо сгорает в ступени
        Mass(n) = Mass(n-1) - dmdt * Delta;                     % Масса ракеты уменьшается
    end

    dvdt = u*dmdt/Mass(n-1) - G*ME/(R+x(n-1))^2;    % Расчет моментального ускорения в момент времени t(n)
    V(n) = V(n-1)+dvdt*Delta;                       % Расчет скорости в момент времени t(n)
    x(n) = x(n-1)+Delta*0.5*(V(n)+V(n-1));          % Расчет высоты в момент времени t(n)

    if V(n) < 0 && n == 2                           % Проверка скорости на старте
        x(n) = 0;                                   % Сможет ли ракета взлететь?
        V(n) = 0;
        disp("Ракета не взлетит!");
        grafs = false;
        break;
    end
end

% Unity data
fileID = fopen('data_u.txt','r');
formatSpec = '%f %f %f %f';
s = [4 Inf];
data_u = fscanf(fileID,formatSpec,s);

if grafs

    figure('units','normalized','outerposition',[0 0 1 1]) 
    
    % График скорости
    subplot(2,2,1)
    hold on
    plot(t(1:n),V(1:n)/1000);
    plot(data_u(4,:),data_u(1,:)/1000);
    legend({'Matlab','Unity'},'Location','northwest');
    xlabel({'Время (с)'});
    ylabel({'V (км/с)'});
    title({'Скорость'});
    hold off


    % График массы
    subplot(2,2,2)
    hold on
    plot(t(1:n),Mass(1:n)/1000);
    plot(data_u(4,:),data_u(2,:)/1000);
    legend('Matlab','Unity');
    xlabel({'Время (с)'});
    ylabel({'Масса (т)'});
    title({'Масса'});
    hold off

    % График высоты
    subplot(2,2,[3, 4])
    hold on
    plot(t(1:n),x(1:n)/1000);
    plot(data_u(4,:),data_u(3,:)/1000);
    legend('Matlab','Unity');
    xlabel({'Время (с)'});
    ylabel({'H (км)'});
    title({'Высота'});
    hold off

end