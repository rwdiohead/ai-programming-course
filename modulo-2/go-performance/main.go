package main

import (
	"fmt"
	"time"
)

func processImage(id int) {
	fmt.Printf("Procesando imagen %d...\n", id)
	time.Sleep(500 * time.Millisecond)
}

func main() {
	start := time.Now()
	totalImages := 10

	fmt.Println("--- Iniciando Procesamiento Secuencial ---")

	for i := 1; i <= totalImages; i++ {
		processImage(i)
	}

	elapsed := time.Since(start)
	fmt.Printf("--- Finalizado en %s ---\n", elapsed)
}
