import { useEffect, useState } from "react";
import TaskCard from "../components/TaskCard";
import api from "../services/api";
import type { Task } from "../types/task";
import { useAuth } from "../context/AuthContext";

export default function Dashboard() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");

  const [editingTask, setEditingTask] = useState<Task | null>(null);

  const { logout } = useAuth();

  async function loadTasks() {
    const response = await api.get("/tasks");
    setTasks(response.data);
  }

  useEffect(() => {
    loadTasks();
  }, []);

  function handleEdit(task: Task) {
    setEditingTask(task);
    setTitle(task.title);
    setDescription(task.description ?? "");
  }

  async function saveTask() {
    if (!title) return;

    if (editingTask) {
      await api.put(`/tasks/${editingTask.id}`, {
        title,
        description,
        dueDate: editingTask.dueDate,
        isCompleted: editingTask.isCompleted,
      });

      setEditingTask(null);
    } else {
      await api.post("/tasks", {
        title,
        description,
        dueDate: new Date().toISOString(),
      });
    }

    setTitle("");
    setDescription("");
    loadTasks();
  }

  async function deleteTask(id: string) {
    await api.delete(`/tasks/${id}`);
    loadTasks();
  }

  async function toggleTask(task: Task) {
    await api.put(`/tasks/${task.id}`, {
      title: task.title,
      description: task.description,
      dueDate: task.dueDate,
      isCompleted: !task.isCompleted,
    });

    loadTasks();
  }

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="flex justify-between items-center mb-8">
        <h1 className="text-3xl font-bold">Tasks</h1>

        <button
          onClick={logout}
          className="bg-red-600 text-white px-4 py-2 rounded"
        >
          Logout
        </button>
      </div>

      <div className="bg-white p-4 rounded shadow mb-8 flex flex-col gap-3">
        <input
          type="text"
          placeholder="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="border p-2 rounded"
        />

        <textarea
          placeholder="Description"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          className="border p-2 rounded"
        />

        <button
          onClick={saveTask}
          className="bg-blue-600 text-white p-2 rounded"
        >
          {editingTask ? "Update Task" : "Create Task"}
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {tasks.map((task) => (
          <TaskCard
            key={task.id}
            task={task}
            onDelete={deleteTask}
            onToggle={toggleTask}
            onEdit={handleEdit}
          />
        ))}
      </div>
    </div>
  );
}
