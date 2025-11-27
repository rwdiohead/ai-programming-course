package main

import (
	"fmt"
	"sync"
	"time"
)

func processImage(id int) {
	fmt.Printf("Procesando imagen %d...\n", id)
	time.Sleep(500 * time.Millisecond)
}

func worker(id int, jobs <-chan int, wg *sync.WaitGroup) {
	defer wg.Done()

	for imageID := range jobs {
		fmt.Printf("Worker %d procesando imagen %d\n", id, imageID)
		processImage(imageID)
	}
}

func main() {
	start := time.Now()
	totalImages := 10
	numWorkers := 5

	fmt.Println("--- Iniciando Procesamiento con Worker Pool ---")

	// Crear canal para distribuir tareas
	jobs := make(chan int, totalImages)

	// WaitGroup para sincronizar workers
	var wg sync.WaitGroup

	// Crear el pool de 5 workers
	for i := 1; i <= numWorkers; i++ {
		wg.Add(1)
		go worker(i, jobs, &wg)
	}

	// Enviar tareas al canal
	for i := 1; i <= totalImages; i++ {
		jobs <- i
	}

	// Cerrar el canal para indicar que no hay más tareas
	close(jobs)

	// Esperar a que todos los workers terminen
	wg.Wait()

	elapsed := time.Since(start)
	fmt.Printf("--- Finalizado en %s ---\n", elapsed)
}
