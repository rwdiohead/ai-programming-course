// Actúa como Senior Backend Dev. Basado en las reglas definidas en @ARCHITECTURE.md, implementa la función createCsvProcessor. Debe retornar un AsyncGenerator que emita objetos validados.


import * as fs from 'fs';
import * as readline from 'readline';
import { z } from 'zod';

export type Result<T, E> =
  | { ok: true; value: T }
  | { ok: false; error: E };

export interface User {
  id: string;
  nombre: string;
  email: string;
}

const userSchema = z.object({
  id: z.string(),
  nombre: z.string(),
  email: z.string().email(),
});

function parseCsvLine(line: string): string[] {
  const fields: string[] = [];
  let cur = '';
  let inQuotes = false;
  for (let i = 0; i < line.length; i++) {
    const ch = line[i];
    if (ch === '"') {
      if (inQuotes && i + 1 < line.length && line[i + 1] === '"') {
        cur += '"';
        i++;
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
 * Crea un AsyncGenerator que emite Result<User, ErrorInfo> para cada fila del CSV.
 * - No lanza excepciones para flujo de control; devuelve Result con ok:false en errores.
 * - Valida cada fila con zod y registra filas inválidas.
 * - Usa streams / readline para no cargar todo el archivo en memoria.
 */
export async function* createCsvProcessor(
  filePath: string,
  options?: { skipHeader?: boolean }
): AsyncGenerator<Result<User, { line: number; errors: unknown; raw: string }>, void, void> {
  const skipHeader = options?.skipHeader ?? true;

  const stream = fs.createReadStream(filePath, { encoding: 'utf8' });
  const rl = readline.createInterface({ input: stream, crlfDelay: Infinity });

  let lineNumber = 0;
  let streamError: unknown | null = null;

  stream.on('error', (err) => {
    streamError = err;
  });

  try {
    for await (const line of rl) {
      lineNumber++;
      if (lineNumber === 1 && skipHeader) continue;

      const raw = line;
      if (!line) {
        // empty line -> skip
        continue;
      }

      const cols = parseCsvLine(line);
      // map to object by position
      const obj = { id: cols[0] ?? '', nombre: cols[1] ?? '', email: cols[2] ?? '' };

      const parsed = userSchema.safeParse(obj);
      if (!parsed.success) {
        const errorInfo = { line: lineNumber, errors: parsed.error.format(), raw };
        console.warn('CSV validation failed at line', lineNumber, parsed.error.errors.map(e => e.message));
        yield { ok: false as const, error: errorInfo };
        continue;
      }

      yield { ok: true as const, value: parsed.data };
    }

    if (streamError) {
      // yield a final error result for stream errors
      yield { ok: false as const, error: { line: -1, errors: streamError, raw: '' } };
    }
  } finally {
    rl.close();
    stream.destroy();
  }
}
