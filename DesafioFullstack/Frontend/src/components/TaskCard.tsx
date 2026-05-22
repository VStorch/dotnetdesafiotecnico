import type { Task } from "../types/task";

type Props = {
  task: Task;
  onDelete: (id: string) => void;
  onToggle: (task: Task) => void;
};

export default function TaskCard({ task, onDelete, onToggle }: Props) {
  return (
    <div className="border rounded p-4 flex flex-col gap-2 bg-white shadow">
      <h2 className="font-bold text-lg">{task.title}</h2>

      <p>{task.description}</p>

      <span>Status: {task.isCompleted ? "Completed" : "Pending"}</span>

      <div className="flex gap-2 mt-2">
        <button
          onClick={() => onToggle(task)}
          className="bg-green-600 text-white px-3 py-1 rounded"
        >
          Toggle
        </button>

        <button
          onClick={() => onDelete(task.id)}
          className="bg-red-600 text-white px-3 py-1 rounded"
        >
          Delete
        </button>
      </div>
    </div>
  );
}
