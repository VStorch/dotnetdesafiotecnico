import { useEffect, useState } from "react";
import TaskCard from "../components/TaskCard";
import api from "../services/api";
import type { Task } from "../types/task";
import { useAuth } from "../context/AuthContext";

type Filter = "all" | "pending" | "completed";

export default function Dashboard() {
  const [tasks, setTasks] = useState<Task[]>([]);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [filter, setFilter] = useState<Filter>("all");

  const [editingTask, setEditingTask] = useState<Task | null>(null);

  const { logout } = useAuth();

  async function loadTasks() {
    try {
      const response = await api.get("/tasks");
      setTasks(response.data);
    } catch {
      console.error("Failed to load tasks");
    }
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

    try {
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
    } catch {
      console.error("Failed to save task");
    }
  }

  async function deleteTask(id: string) {
    try {
      await api.delete(`/tasks/${id}`);
      loadTasks();
    } catch {
      console.error("Failed to delete task");
    }
  }

  async function toggleTask(task: Task) {
    try {
      await api.put(`/tasks/${task.id}`, {
        title: task.title,
        description: task.description,
        dueDate: task.dueDate,
        isCompleted: !task.isCompleted,
      });

      loadTasks();
    } catch {
      console.error("Failed to toggle task");
    }
  }

  const filteredTasks = tasks.filter((task) => {
    if (filter === "pending") return !task.isCompleted;
    if (filter === "completed") return task.isCompleted;
    return true;
  });

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

      <div className="flex gap-2 mb-4">
        {(["all", "pending", "completed"] as Filter[]).map((f) => (
          <button
            key={f}
            onClick={() => setFilter(f)}
            className={`px-4 py-1 rounded capitalize ${
              filter === f
                ? "bg-blue-600 text-white"
                : "bg-white text-gray-700 border"
            }`}
          >
            {f === "all" ? "All" : f === "pending" ? "Pending" : "Completed"}
          </button>
        ))}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {filteredTasks.map((task) => (
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
