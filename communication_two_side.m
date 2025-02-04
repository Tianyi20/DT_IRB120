clc
clear

syms theta_1 theta_2 theta_3 theta_4 theta_5 theta_6;

%% IRB12O standard DH 建模

%%%%%尤其注意%%%%%

% theta % d $ a % alpha
DH = [0, 290, 0, -pi/2;
    0, 0, 270, 0;
    0, 0, 70, -pi/2;
    0, 302, 0, pi/2;
    0, 0, 0, -pi/2;
    0, 72, 0, 0
    ];

L1=Link(DH(1,:),'standard');
L2=Link(DH(2,:),'standard');
L3=Link(DH(3,:),'standard');
L4=Link(DH(4,:),'standard');
L5=Link(DH(5,:),'standard');
L6=Link(DH(6,:),'standard');
robot=SerialLink([L1,L2,L3,L4,L5,L6]);
robot.name='Manipulator';

%%初始化transformation matrix,不然会报错
p_tool = zeros(4,4);


%% socket 初始化
%%通道一读数据
tcpServer = tcpip('0.0.0.0', 55000, 'NetworkRole', 'server','timeout',50);
% tcpServer.InputBufferSize = 10000; % or a value that suits your data size
% tcpserver.timeout = 20000000000000000000000000000;
%%通道2传数据
tcpServer_send = tcpip('0.0.0.0', 65000, 'NetworkRole', 'server');
tcpServer_send.InputBufferSize = 10000;

%打开socket
fopen(tcpServer);

disp('Socket connection established successfully.');
fopen(tcpServer_send);

disp('Socket_send connection established successfully.');

%% TCP初始化和intial guess的值初始化，计数器初始化
TCP_X_receive = 0;
TCP_Y_receive = 0;
TCP_Z_receive = 0;
TCP_X_receive_last = 0;
TCP_Y_receive_last = 0;
TCP_Z_receive_last = 0;
Real_angle_deg = zeros(1, 6);
theta_temp_guess = [0, 0, 0, 0, 90, 0;];
Joint_value = zeros(1, 6);
j = 0;

%% 读取unity的数据，嵌套一个大循环，一直读取，如果数据发现有变化则计算逆运动学

while true
    %%加一个计数器
    j = j + 1;
    %%从unity接受数据

    try
        data = fscanf(tcpServer);

        display(data)
        %disp(data);

    catch exception
        fprintf('读取数据时发生错误：%s\n', exception.message);
    end

    %%检索数据;先检索TCP
    Ac = data == 'A'; indxA = find(Ac); Ax = indxA(1);
    Bc = data == 'B'; indxB = find(Bc); Bxx = indxB(indxB > Ax); Bx = Bxx(1);
    Cc = data == 'C'; indxC = find(Cc); Cxx = indxC(indxC > Bx); Cx = Cxx(1);
    Dc = data == 'D'; indxD = find(Dc); Dxx = indxD(indxD > Cx); Dx = Dxx(1);
    %%检索Joint value
    Ec = data == 'E'; indxE = find(Ec); Exx = indxE(indxE > Dx); Ex = Exx(1);
    Fc = data == 'F'; indxF = find(Fc); Fxx = indxF(indxF > Ex); Fx = Fxx(1);
    Gc = data == 'G'; indxG = find(Gc); Gxx = indxG(indxG > Fx); Gx = Gxx(1);
    Hc = data == 'H'; indxH = find(Hc); Hxx = indxH(indxH > Gx); Hx = Hxx(1);
    Ic = data == 'I'; indxI = find(Ic); Ixx = indxI(indxI > Hx); Ix = Ixx(1);
    Jc = data == 'J'; indxJ = find(Jc); Jxx = indxJ(indxJ > Ix); Jx = Jxx(1);

    %%TCP vlalue
    TCP_X_receive_new = str2double(data(Ax + 1:Bx - 1));
    TCP_Y_receive_new = str2double(data(Bx + 1:Cx - 1));
    TCP_Z_receive_new = str2double(data(Cx + 1:Dx - 1));
    %%保存每一次的joint的值，覆盖
    Joint_value(1,1) = str2double(data(Dx + 1:Ex - 1));
    Joint_value(1,2) = str2double(data(Ex + 1:Fx - 1));
    Joint_value(1,3) = str2double(data(Fx + 1:Gx - 1));
    Joint_value(1,4) = str2double(data(Gx + 1:Hx - 1));
    Joint_value(1,5) = str2double(data(Hx + 1:Ix - 1));
    Joint_value(1,6) = str2double(data(Ix + 1:Jx - 1));
    %%把角度值换成弧度制
    Joint_value = deg2rad(Joint_value);
    theta_temp_guess = [Joint_value(1,1), Joint_value(1,2) - pi/2,Joint_value(1,3),Joint_value(1,4), Joint_value(1,5),Joint_value(1,6)];
    %%robot.plot(theta_temp_guess);

    %检查和上一次传回来的值有没有变化，不然浪费matlab算力了

    %%正运动学,theta_temp_guess是猜出来的值，也是当前的关节值
    p_tool = double(robot.fkine(theta_temp_guess));
    %%逆运动学
    %给从unity传过来的TCP的X Y Z
    %%% TCP 赋值
    p_tool(1,4) = TCP_X_receive_new;
    p_tool(2,4) = TCP_Y_receive_new;
    p_tool(3,4) = TCP_Z_receive_new;
    fprintf('After substituting the position into inverse kinematics, the expected angles are:\n')
    %调用逆运动学，算六个DOF，注意这里面需要一个guess value,theta_temp_guess
    theta_tool = robot.ikine(p_tool,'ilimit',1000, 'tol', 1e-6, 'q0', theta_temp_guess, 'verbose','mask',[1 1 1 1 1 1]);
    Real_theta_output = theta_tool;
    % 模拟的theta2 应该等于实际的 (theta2 - pi/2)
    Real_theta_output(1,2) = Real_theta_output(1,2) + pi/2;
    %把结果转化成角度值
    Real_angle_deg = rad2deg(Real_theta_output);
    display(Real_angle_deg);


    % Send Joint values to Unity
    fprintf(tcpServer_send, 'A%fB%fC%fD%fE%fF%fG\n', [Real_angle_deg(1,1), Real_angle_deg(1,2), Real_angle_deg(1,3),Real_angle_deg(1,4),Real_angle_deg(1,5),Real_angle_deg(1,6)]);




end

% % Stop the server when done
% fclose(tcpServer);
% delete(tcpServer);




