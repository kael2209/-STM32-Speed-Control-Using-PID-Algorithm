#include "main.h"

char pos[30],vel[30];
char Rx_indx,Rx_Buffer[20],Rx_data[2];
int run=0;
float DesiredPos,DesiredSpeed;

float RevCnt=0, OldPos =0, CurPos=0, CurVel=0,delta=0;
uint16_t CurPulses=0,OldPulses=0;
int pwm=0;
float tmp=0;

//volatile float Kp=0,Ki=0,Kd=0,Ts=0.01,alpha=0;
volatile float Kp=0.55,Ki=4.9,Kd=0.003,Ts=0.01,alpha=0.001,a=15;

// Velocity filter
float velocities[10] = {0}; 
uint8_t index = 0;
volatile float sum=0;

int main(void)
{
	systick_init();
	PWM_init();
	encoder_init();
	encoder_enable();
	dma_channel1_4_init(&CurPulses);
	dma_channel1_4_enable();
	usart_init(2);
	tim3_init();
	tim3_enable();
	direction_init();
	RCC->APB2ENR|=(1<<4);
	GPIOC->CRH|=(1<<20);
	while(1)
	{
	}
}
void TIM3_IRQHandler(void)
{
	TIM3->SR&=~(1<<0);
	CurPos = (float)CurPulses/800.0 * 2 *PI;
	if (CurPulses>=OldPulses)
	{
		delta  =CurPulses - OldPulses;
	}
	else if (CurPulses<OldPulses)
	{
		delta = (65535-OldPulses)+CurPulses+1;
	}
	OldPulses = CurPulses;
	CurVel = (delta/400*PI)/0.01;	// 10ms
	
	//dir(1);
	//pwm = DesiredSpeed;
	//pwm = PID_Velocity(PID_Position(CurPos,DesiredPos,DesiredSpeed,a),CurVel,Kp,Ki,Kd,Ts,alpha);
	
	
	/* CONTROL VELOCITY OF DC SERVO MOTOR */
	dir(1);
	pwm = PID_Velocity(DesiredSpeed,CurVel,Kp,Ki,Kd,Ts,alpha);
	
	TIM4->CCR3 =375*pwm;
	
	velocities[index] = CurVel;
	if (index==9)
	{
		sum=0;
    for (uint8_t i = 0; i < 10; i++) {
        sum += velocities[i];
    }
    sum /= 10;
		sprintf(pos,"P%.2f\r \n",tmp);//CurPos);	// Gui UART
		usart_send(2,pos);
		sprintf(vel,"V%.2f\r \n",sum);
		usart_send(2,vel);
		sum=0;
		index=0;
	}
	else index++;
}

void USART2_IRQHandler(void)
{
	Rx_data[0] = USART2->DR;
	if(Rx_indx==0) 
	{
		for (uint8_t i=0;i<20;i++) 
		{
			Rx_Buffer[i] = 0; 
		}
	}     
	switch(Rx_data[0]) 
	{
		case 'e': run = false; break; 
		case 'r': run = true; break;  
		case 's': 
			DesiredPos = atoi(Rx_Buffer); 
			memset(Rx_Buffer, 0, sizeof(Rx_Buffer));
			Rx_indx = 0;
			break;
		case 'v': 
			DesiredSpeed = atoi(Rx_Buffer); 
			memset(Rx_Buffer, 0, sizeof(Rx_Buffer));
			Rx_indx = 0;
			break;
		case '0'...'9': case '.': case '-':
			Rx_Buffer[Rx_indx++] |= Rx_data[0];
			break;
		default: 
			break;
	}
}

/**/
