#include "stm32f10x.h"     
#include "dma.h"


void dma_channel1_4_init(uint16_t* counter)
{
	RCC->AHBENR|=RCC_AHBENR_DMA1EN;
	DMA1_Channel5->CPAR = (uint32_t)&TIM2->CNT;
	DMA1_Channel5->CMAR = (uint32_t)counter;
	DMA1_Channel5->CNDTR = 2;
	DMA1_Channel5->CCR|=(1<<8)|(1<<10)|(1<<7)|(1<<5);
	DMA1_Channel7->CPAR = (uint32_t)&TIM2->CNT;
	DMA1_Channel7->CMAR = (uint32_t)counter;
	DMA1_Channel7->CNDTR = 2;
	DMA1_Channel7->CCR|=(1<<8)|(1<<10)|(1<<7)|(1<<5);
}
void dma_channel1_4_enable(void)
{
	DMA1_Channel5->CCR|=(1<<0);
	DMA1_Channel7->CCR|=(1<<0);
}
