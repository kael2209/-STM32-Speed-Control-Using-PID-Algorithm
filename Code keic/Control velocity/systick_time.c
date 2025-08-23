#include "stm32f10x.h"           
#include "systick_time.h"

void systick_init(void)
{
	SysTick->CTRL=0;
	SysTick->LOAD=0xFFFFFF;
	SysTick->VAL=0;
	SysTick->CTRL=5;
}
void delaymillis(void)
{
	SysTick->LOAD=72000;
	SysTick->VAL=0;
	while(!(SysTick->CTRL&(1<<16)));
}
void delay(int t)
{
	for (;t>0;t--)
	{
		delaymillis();
	}
}
void systick_int_init(void)
{
	SysTick->CTRL=0;
	SysTick->LOAD=72000;
	SysTick->VAL=0;
	SysTick->CTRL=7;
}
void systick_int_start(unsigned short usart1_mgr[],unsigned short usart2_mgr[],volatile unsigned short usart3_mgr[])
{
	if (usart1_mgr[0]!=0)
	{
		if (usart1_mgr[6]==0)
		{
			usart1_mgr[0]=0;
			usart1_mgr[1]=1;
			usart1_mgr[5]=0;
			systick_init();
		}
		else usart1_mgr[6]--;
	}
	else if (usart2_mgr[0]!=0)
	{
		if (usart2_mgr[6]==0)
		{
			usart2_mgr[0]=0;
			usart2_mgr[1]=1;
			usart2_mgr[5]=0;
			systick_init();
		}
		else usart2_mgr[6]--;
	}
	else if (usart3_mgr[0]!=0)
	{
		if (usart3_mgr[6]==0)
		{
			usart3_mgr[0]=0;
			usart3_mgr[1]=1;
			usart3_mgr[5]=0;
			systick_init();
		}
		else usart3_mgr[6]--;
	}
}
