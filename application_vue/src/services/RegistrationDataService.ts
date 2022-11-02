import http from "@/http-common";
import Student from "@/types/Student";

class TutorialDataService {
  getAll(): Promise<any> {
    return http.get("/student");
  }

  get(hash_id: string): Promise<any> {
    return http.get(`/student/${hash_id}`);
  }

  create(data: Student): Promise<any> {
    return http.post("/student", data);
  }

  update(data: Student): Promise<any> {
    return http.put(`/student`, data);
  }

  delete(hash_id: string): Promise<any> {
    return http.delete(`/student/${hash_id}`);
  }

  deleteAll(): Promise<any> {
    return http.delete(`/student`);
  }

  findByName(nome: string): Promise<any> {
    return http.get(`/student/FindByName`, {headers: {'nome': nome}});
  }
}

export default new TutorialDataService();