#include "stm32f10x.h" 
#include "direction.h"

void direction_init(void)
{
	// PA11 PA12
	RCC->APB2ENR|=(1<<2);
	GPIOA->CRH=0;
	GPIOA->CRH|=(1<<12)|(1<<16);
	GPIOA->ODR&=~(1<<11)&~(1<<12);
}

void dir(int direction)
{
	if (direction==1)
	{
		GPIOA->ODR&=~(1<<11);
		GPIOA->ODR|=(1<<12);
	}
	else 
	{
		GPIOA->ODR&=~(1<<12);
		GPIOA->ODR|=(1<<11);
	}
}
