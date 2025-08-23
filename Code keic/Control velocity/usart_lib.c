#include "stm32f10x.h"           
#include "usart_lib.h"

void usart_init(unsigned short usart)
{
	if (usart==1)
	{
		RCC->APB2ENR|=(1<<0)|(1<<2)|(1<<14);
		GPIOA->CRH=0;
		GPIOA->CRH|=(11<<4)|(8<<8);
		USART1->BRR = 0x271;
		USART1->CR1|=(1<<2)|(1<<3)|(1<<5)|(1<<13);	
	}
	else if (usart==2)
	{
		RCC->APB1ENR|=(1<<17);
		RCC->APB2ENR|=(1<<0)|(1<<2);
		//GPIOA->CRL=0;
		GPIOA->CRL|=(11<<8)|(8<<12);
		USART2->BRR = 0x139;
		USART2->CR1|=(1<<2)|(1<<3)|(1<<5)|(1<<13);
		NVIC->ISER[1]|=(1<<6);
	}
	else if (usart==3)
	{
		RCC->APB1ENR|=(1<<18);
		RCC->APB2ENR|=(1<<0)|(1<<3);
		GPIOB->CRH=0;
		GPIOB->CRH|=(11<<8)|(8<<12);
		USART3->BRR = 0x139;
		USART3->CR1|=(1<<2)|(1<<3)|(1<<5)|(1<<13);
	}
}

void usart_tx(unsigned short usart, char ch)
{
	if (usart==2)
	{
		while(!(USART2->SR&(1<<6)));
		USART2->DR = ch;
	}
	else if (usart==3)
	{
		while(!(USART3->SR&(1<<6)));
		USART3->DR = ch;
	}
}

char usart_rx(unsigned short usart)
{
	char ch;
	if (usart==2)
	{
		while(!(USART2->SR&(1<<5)));
		ch = USART2->DR;
	}
	else if (usart==3)
	{
		while(!(USART3->SR&(1<<5)));
		ch = USART3->DR;
	}
	return ch;
}

void usart_send(unsigned short usart, char str[])
{
		int i=0;
		while(str[i])
		{
			usart_tx(usart,str[i]);
			i++;
		}
}






