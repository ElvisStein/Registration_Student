<template>
  <div class="card bg-light pt-3 pb-3">
    <div class="col-md-12">
      <div class="input-group mb-3">
        <input
          type="text"
          class="form-control"
          placeholder="Buscar pelo Nome"
          v-model="nome"
        />
        <div class="input-group-append">
          <button
            class="btn btn-outline-secondary"
            type="button"
            @click="searchName"
          >
            Buscar
          </button>
        </div>
        <div class="col-md-2 text-right">
          <router-link :to="'/add'" class="btn btn-success"
            >Cadastrar Aluno</router-link
          >
        </div>
      </div>
    </div>
    <div class="card-body row">
      <div class="col-md-6">
        <h4>Estudantes</h4>
        <ul class="list-group">
          <li
            class="list-group-item"
            :class="{ active: index == currentIndex }"
            v-for="(student, index) in students"
            :key="index"
            @click="setActiveStudent(student, index)"
          >
            {{ student.nome }} - {{student.cpf}}
          </li>
        </ul>
      </div>
      <div class="col-md-4">
        <h4>Dados:</h4>
        <div class="card p-3">
          <div v-if="currentStudent.hashId">
            <div>
              <label><strong>Nome:</strong></label> {{ currentStudent.nome }}
            </div>
            <div>
              <label><strong>CPF:</strong></label>
              {{ currentStudent.cpf }}
            </div>
            <div>
              <label><strong>E-mail:</strong></label>
              {{ currentStudent.email }}
            </div>

            <div class="row pl-3 pr-3">
              <router-link
                :to="'/student/' + currentStudent.hashId"
                class="btn btn-warning mr-2"
                >Editar</router-link
              >
              <button class="btn btn-danger" @click="handleDeleteStudent">
                Deletar
              </button>
            </div>
          </div>
          <div v-else>
            <p>Selecione um estudante...</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
  
  <script lang="ts">
import { defineComponent } from "vue";
import RegistrationDataService from "@/services/RegistrationDataService";
import Student from "@/types/Student";
import ResponseData from "@/types/ResponseData";
import Swal from "sweetalert2/dist/sweetalert2.js";

const swalWithBootstrapButtons = Swal.mixin({
  customClass: {
    confirmButton: "btn btn-success",
    cancelButton: "btn btn-danger mr-3",
  },
  buttonsStyling: false,
});

export default defineComponent({
  name: "students-list",
  data() {
    return {
      students: [] as Student[],
      currentStudent: {} as Student,
      currentIndex: -1,
      nome: "",
    };
  },
  methods: {
    retrievestudents() {
      RegistrationDataService.getAll()
        .then((response: ResponseData) => {
          this.students = response.data;
          console.log(response.data);
        })
        .catch((e: Error) => {
          console.log(e);
        });
    },

    refreshList() {
      this.retrievestudents();
      this.currentStudent = {} as Student;
      this.currentIndex = -1;
    },

    setActiveStudent(student: Student, index = -1) {
      console.log(student);
      this.currentStudent = student;
      this.currentIndex = index;
    },

    searchName() {
      if (this.nome != "") {
        RegistrationDataService.findByName(this.nome)
          .then((response: ResponseData) => {
            this.students = response.data;
            this.setActiveStudent({} as Student);
            console.log(response.data);
          })
          .catch((e: Error) => {
            console.log(e);
          });
      } else this.retrievestudents();
    },

    handleDeleteStudent() {
      swalWithBootstrapButtons
        .fire({
          title: "Atenção",
          text: "Deseja deletar este estudante!",
          icon: "warning",
          showCancelButton: true,
          confirmButtonText: "Sim, Deletar",
          cancelButtonText: "Não, Cancelar!",
          reverseButtons: true,
        })
        .then(async (result) => {
          if (result.isConfirmed) {
            if (await this.deleteStudent()) {
              this.refreshList();
              swalWithBootstrapButtons.fire(
                "Estudante deletado com sucesso!",
                "",
                "success"
              );
            } else {
              swalWithBootstrapButtons.fire(
                "Erro!",
                "Ocorreu um erro ao deletar o estudante!",
                "error"
              );
            }
          } else if (
            /* Read more about handling dismissals below */
            result.dismiss === Swal.DismissReason.cancel
          ) {
            swalWithBootstrapButtons.fire(
              "Operação cancelada",
              "Estudante não foi deletado",
              "error"
            );
          }
        });
    },

    deleteStudent(): Promise<Boolean> {
      return RegistrationDataService.delete(this.currentStudent.hashId)
        .then((response: ResponseData) => {
          console.log(response.data);
          return true;
        })
        .catch((e: Error) => {
          console.log(e);
          return false;
        });
    },
  },
  mounted() {
    this.retrievestudents();
  },
});
</script>
  
  <style>
.list {
  text-align: left;
  max-width: 750px;
  margin: auto;
}
</style>