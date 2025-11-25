// Escribe una función en TypeScript para leer un archivo CSV grande de usuarios (id, nombre, email) y procesarlos

import * as fs from 'fs';
import * as readline from 'readline';
import * as path from 'path';

export interface User {
	id: string;
	nombre: string;
	email: string;
}

function parseCsvLine(line: string): string[] {
	const fields: string[] = [];
	let cur = '';
	let inQuotes = false;
	for (let i = 0; i < line.length; i++) {
		const ch = line[i];
		if (ch === '"') {
			// If double quote inside quoted field is escaped by another quote, consume it
			if (inQuotes && i + 1 < line.length && line[i + 1] === '"') {
				cur += '"';
				i++; // skip the escaped quote
			} else {
				inQuotes = !inQuotes;
			}
		} else if (ch === ',' && !inQuotes) {
			fields.push(cur);
			cur = '';
		} else {
			cur += ch;
		}
	}
	fields.push(cur);
	return fields.map(f => f.trim());
}

/**
 * Procesa un CSV grande de usuarios (id, nombre, email) usando streams.
 * - `onUser` se llama por cada usuario (puede devolver Promise para operaciones async).
 * - `batchSize` agrupa usuarios para procesarlos en lotes (por defecto 100).
 * - Por simplicidad este parser NO maneja saltos de línea dentro de campos entrecomillados.
 */
export async function processLargeCsv(
	filePath: string,
	onUser: (user: User) => Promise<void> | void,
	options?: { batchSize?: number; skipHeader?: boolean }
): Promise<void> {
	const batchSize = options?.batchSize ?? 100;
	const skipHeader = options?.skipHeader ?? true;

	const stream = fs.createReadStream(filePath, { encoding: 'utf8' });
	const rl = readline.createInterface({ input: stream, crlfDelay: Infinity });

	let lineNumber = 0;
	let batch: User[] = [];

	async function flushBatch() {
		if (batch.length === 0) return;
		const tasks = batch.map(u => Promise.resolve().then(() => onUser(u)));
		const results = await Promise.allSettled(tasks);
		for (const r of results) {
			if (r.status === 'rejected') {
				console.error('Error processing user batch item:', r.reason);
			}
		}
		batch = [];
	}

	try {
		for await (const line of rl) {
			lineNumber++;
			if (lineNumber === 1 && skipHeader) continue;
			if (!line) continue;

			const cols = parseCsvLine(line);
			if (cols.length < 3) {
				console.warn(`Skipping malformed line ${lineNumber}: ${line}`);
				continue;
			}

			const user: User = { id: cols[0], nombre: cols[1], email: cols[2] };
			batch.push(user);

			if (batch.length >= batchSize) {
				await flushBatch();
			}
		}

		await flushBatch();
	} finally {
		rl.close();
		// ensure stream is closed
		stream.destroy();
	}
}

// Ejemplo de uso. Coloca un CSV en `../data/users.csv` con cabecera `id,nombre,email`.
if (require.main === module) {
	(async () => {
		const demoPath = path.join(__dirname, '..', 'data', 'users.csv');
		try {
			await processLargeCsv(demoPath, async (u) => {
				// Ejemplo: procesar cada usuario (aquí solo lo mostramos)
				console.log(`Procesado: ${u.id} - ${u.nombre} <${u.email}>`);
			}, { batchSize: 200, skipHeader: true });
		} catch (err) {
			console.error('Error al procesar CSV:', err);
		}
	})();
}

