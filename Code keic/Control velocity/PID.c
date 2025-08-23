#include "PID.h"
#include <math.h>
#include "direction.h"

int PID_Velocity(float DesiredValue, float CurrentValue,float Kp,float Ki,float Kd,float Ts,float alpha)
{
	static float err_p=0, ui_p=0,udf_p=0;
	float err,up,ui,ud,udf;
	int uout=0;
	
	err = DesiredValue - CurrentValue;
	
	up = Kp*err;
	ui = ui_p + Ki*Ts*err;
	ud = Kd*(err - err_p)/Ts;
	udf = (1-alpha)*udf_p + alpha*ud;
	
	ui_p = ui;
	err_p = err;
	udf_p = udf;
	
	uout = (int)(up+ui+ud);
	if (uout>=100) uout=100;
	else if (uout<28) uout=0;
	return uout;
}
float PID_Position(float CurrentPos, float DesiredPos,float DesiredVel, float a)
{
	static float InitialPos = 0.0;
	static char flag = 1;
	float RefVel,s_accel,s_decel;
	if (flag) 
	{
		InitialPos = CurrentPos;
		flag = 0;	
	}
	// Quãng du?ng tang t?c và gi?m t?c
	float accel_distance = (DesiredVel * DesiredVel - 100)/ (2*a);
	
	// Ki?m tra có giai do?n v?n t?c ko d?i hay ko
	float total_distance = DesiredPos - InitialPos;
	float min_distance = 2 * accel_distance;
	float direction = (total_distance>=0)? 1.0:-1.0;
		
	// Các di?m chuy?n ti?p
	if (direction>0)	// Ti?n v? phía tru?c
	{
		s_accel = InitialPos + accel_distance;
		s_decel = DesiredPos - accel_distance;
	}
	else if (direction<0)	// Lùi v?
	{
		s_accel = InitialPos - accel_distance;
		s_decel = DesiredPos + accel_distance;
	}
	
	if (min_distance>=fabs(total_distance))	// Có 2 giai do?n
	{
	}
	else if (min_distance< fabs(total_distance)) //  CO 3 GIAI DOAN
	{
		if (direction>0)	// TIEN LEN
		{
				dir(1);
				if (CurrentPos < s_accel)
				{
					RefVel = sqrt(2*a*(CurrentPos-InitialPos)+100);
					if (RefVel> DesiredVel) RefVel = DesiredVel;
				}
				else if (CurrentPos < s_decel)
				{
					RefVel = DesiredVel;
				}
				else if (CurrentPos < (DesiredPos-1))
				{
					float remaining = DesiredPos - CurrentPos;
					RefVel = sqrt(2*a*remaining+100);
					if (RefVel> DesiredVel) RefVel = DesiredVel;	
				}
				else
				{
					RefVel =0;
					flag=1;
				}
		}
		else if (direction<0)	// LUI LAI
		{
				dir(-1);
				if (CurrentPos > s_accel)
				{
					RefVel = sqrt(2*a*(-CurrentPos+InitialPos)+100);
					if (RefVel> DesiredVel) RefVel = DesiredVel;
				}
				else if (CurrentPos > s_decel)
				{
					RefVel = DesiredVel;
				}
				else if (CurrentPos > DesiredPos+1)
				{
					float remaining = -DesiredPos + CurrentPos;
					RefVel = sqrt(2*a*remaining+100);
					if (RefVel> DesiredVel) RefVel = DesiredVel;	
				}
				else
				{
					RefVel =0;
					flag=1;
				}
		}
			
	}
	return RefVel;
}
