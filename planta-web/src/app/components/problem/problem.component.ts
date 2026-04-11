import { Component } from '@angular/core';
import { ScrollAnimateDirective } from '../../shared/directives/scroll-animate.directive';

interface PainPoint {
  icon: string;
  title: string;
  description: string;
}

@Component({
  selector: 'app-problem',
  standalone: true,
  imports: [ScrollAnimateDirective],
  templateUrl: './problem.component.html',
  styleUrl: './problem.component.scss',
})
export class ProblemComponent {
  readonly painPoints: PainPoint[] = [
    {
      icon: '📱',
      title: 'Pierdes incidencias entre llamadas y WhatsApp',
      description: 'Los avisos llegan por distintos canales y no hay forma de hacer seguimiento. Las incidencias críticas se pierden entre mensajes.',
    },
    {
      icon: '📅',
      title: 'No sabes cuándo toca el próximo mantenimiento',
      description: 'Sin un sistema centralizado, los mantenimientos preventivos se olvidan y los correctivos se disparan.',
    },
    {
      icon: '👷',
      title: 'Tus técnicos no reportan correctamente',
      description: 'El trabajo realizado queda solo en la memoria del técnico o en papeles que nadie puede encontrar después.',
    },
    {
      icon: '📄',
      title: 'Todo está en Excel, papel o en la cabeza de alguien',
      description: 'El conocimiento operativo es frágil, no está actualizado y se pierde cuando alguien se va de la empresa.',
    },
  ];
}
